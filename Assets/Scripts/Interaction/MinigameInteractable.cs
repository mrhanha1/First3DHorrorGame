using Cinemachine;
using UnityEngine;

public class MinigameInteractable : InteractableBase
{
    [Header("Minigame Settings")]
    [SerializeField] private MinigameBase minigame;
    [SerializeField] private bool lockAfterCompleted = false;
    [SerializeField] private string completedPromptText = "Đã giải rồi";

    [Header("Requirements")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemID;
    [SerializeField] private bool consumeItemOnUse = false;

    [Header("Reward Type")]
    [SerializeField] private RewardType rewardType = RewardType.None;

    [Header("Item Reward")]
    [SerializeField] private string rewardItemID;
    [SerializeField] private Sprite rewardIcon;

    [Header("Environment Reward")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    [Header("Stone Door Reward")]
    [SerializeField] private StoneDoorInteractable stoneDoor;

    private IMinigameService minigameService;
    private IInventoryService inventoryService;
    private bool isCompleted = false;

    public enum RewardType
    {
        None,
        Item,
        Environment,
        StoneDoor
    }

    protected override void Awake()
    {
        base.Awake();
        minigameService = ServiceLocator.Get<IMinigameService>();
        inventoryService = ServiceLocator.Get<IInventoryService>();

        if (minigame == null)
        {
            minigame = GetComponentInChildren<MinigameBase>();
            if (minigame == null)
                Debug.LogError($"[MinigameInteractable] no minigame assigned on {gameObject.name}");
        }

        ValidateRewardSettings();
    }

    private void ValidateRewardSettings()
    {
        // Cảnh báo nếu thiếu settings cho reward type đã chọn
        switch (rewardType)
        {
            case RewardType.Item:
                if (string.IsNullOrEmpty(rewardItemID))
                    Debug.LogWarning($"[MinigameInteractable] {gameObject.name}: Reward type is Item but rewardItemID is empty");
                break;

            case RewardType.Environment:
                if (rewardPrefab == null)
                    Debug.LogWarning($"[MinigameInteractable] {gameObject.name}: Reward type is Environment but rewardPrefab is null");
                break;

            case RewardType.StoneDoor:
                if (stoneDoor == null)
                    Debug.LogWarning($"[MinigameInteractable] {gameObject.name}: Reward type is StoneDoor but stoneDoor is null");
                break;
        }
    }

    public override bool CanInteract(PlayerInteractionController player)
    {
        if (isCompleted && lockAfterCompleted) return false;
        if (requiresItem && inventoryService != null)
        {
            return inventoryService.HasItem(requiredItemID);
        }
        return true;
    }

    public override string getPromptText()
    {
        if (isCompleted && lockAfterCompleted)
            return completedPromptText;
        if (requiresItem && inventoryService != null && !inventoryService.HasItem(requiredItemID))
            return $"Cần: {requiredItemID}";
        return promptText;
    }

    public override void OnInteract(PlayerInteractionController player)
    {
        if (!CanInteract(player))
        {
            var uiService = ServiceLocator.Get<IUIService>();
            uiService?.ShowMessage("Không thể chơi lúc này");
            return;
        }

        if (requiresItem && consumeItemOnUse)
        {
            inventoryService?.RemoveItem(requiredItemID);
        }

        PlaySound(interactSound);
        StartMinigame();
    }

    private void StartMinigame()
    {
        if (minigameService == null)
        {
            Debug.LogError("[MinigameInteractable] Minigame Service not found");
            return;
        }
        if (minigame == null)
        {
            Debug.LogError("[MinigameInteractable] No minigame assigned");
            return;
        }

        minigameService.StartMinigame(minigame, OnMinigameComplete);
    }

    private void OnMinigameComplete()
    {
        bool success = true; // =================== GIAI QUYET SAU ======================
        if (success)
        {
            OnMinigameSuccess();
        }
        else
        {
            OnMinigameFailure();
        }
    }

    private void OnMinigameSuccess()
    {
        isCompleted = true;

        switch (rewardType)
        {
            case RewardType.Item:
                GiveItemReward();
                break;

            case RewardType.Environment:
                GiveEnvironmentReward();
                break;

            case RewardType.StoneDoor:
                UnlockStoneDoor();
                break;

            case RewardType.None:
                break;
        }

        var uiService = ServiceLocator.Get<IUIService>();
        uiService?.ShowMessage("Làm được rồi", 2f);
    }

    private void OnMinigameFailure()
    {
        var uiService = ServiceLocator.Get<IUIService>();
        uiService?.ShowMessage("Vẫn chưa được", 2f);
    }

    private void GiveItemReward()
    {
        if (inventoryService != null && !string.IsNullOrEmpty(rewardItemID))
        {
            inventoryService.AddItem(rewardItemID);
            var uiService = ServiceLocator.Get<IUIService>();
            uiService?.ShowItemPickup(rewardItemID, rewardIcon);
            Debug.Log($"[MinigameInteractable] Item reward given: {rewardItemID}");
        }
        else
        {
            Debug.LogWarning("[MinigameInteractable] Cannot give item reward - inventory service or rewardItemID is null");
        }
    }

    private void GiveEnvironmentReward()
    {
        if (rewardPrefab != null)
        {
            Vector3 spawnPos = rewardSpawnPoint != null
                ? rewardSpawnPoint.position
                : transform.position + Vector3.up;

            GameObject spawnedObject = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[MinigameInteractable] Environment reward spawned: {rewardPrefab.name}");
        }
        else
        {
            Debug.LogWarning("[MinigameInteractable] Cannot spawn environment reward - rewardPrefab is null");
        }
    }

    private void UnlockStoneDoor()
    {
        if (stoneDoor != null)
        {
            var player = FindObjectOfType<PlayerInteractionController>();
            if (player != null)
            {
                stoneDoor.OnInteract(player);
                Debug.Log($"[MinigameInteractable] Stone door opened automatically");

                var uiService = ServiceLocator.Get<IUIService>();
                uiService?.ShowMessage("Cửa đá đã mở!", 2f);
            }
            else
            {
                Debug.LogWarning("[MinigameInteractable] Cannot open stone door - player not found");
            }
        }
        else
        {
            Debug.LogWarning("[MinigameInteractable] Cannot open stone door - stoneDoor reference is null");
        }
    }

    private void OnValidate()
    {
        ValidateRewardSettings();
    }
}