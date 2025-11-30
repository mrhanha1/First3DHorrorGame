using UnityEngine;
/// <summary>
/// Quản lý phần thưởng khi hoàn thành minigame
/// </summary>
public class MinigameReward : MonoBehaviour
{
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

    [Header("Messages")]
    [SerializeField] private string successMessage = "Làm được rồi!";
    [SerializeField] private string failureMessage = "Vẫn chưa được";

    private IInventoryService inventoryService;
    private IUIService uiService;

    public enum RewardType
    {
        None,
        Item,
        Environment,
        StoneDoor
    }

    private void Awake()
    {
        inventoryService = ServiceLocator.Get<IInventoryService>();
        uiService = ServiceLocator.Get<IUIService>();

        ValidateRewardSettings();
    }

    private void ValidateRewardSettings()
    {
        switch (rewardType)
        {
            case RewardType.Item:
                if (string.IsNullOrEmpty(rewardItemID))
                    Debug.LogWarning($"[MinigameReward] {gameObject.name}: Reward type is Item but rewardItemID is empty");
                break;

            case RewardType.Environment:
                if (rewardPrefab == null)
                    Debug.LogWarning($"[MinigameReward] {gameObject.name}: Reward type is Environment but rewardPrefab is null");
                break;

            case RewardType.StoneDoor:
                if (stoneDoor == null)
                    Debug.LogWarning($"[MinigameReward] {gameObject.name}: Reward type is StoneDoor but stoneDoor is null");
                break;
        }
    }

    /// <summary>
    /// Gọi khi minigame hoàn thành thành công
    /// </summary>
    public void GiveReward()
    {
        switch (rewardType)
        {
            case RewardType.Item:
                GiveItemReward();
                break;

            case RewardType.Environment:
                GiveEnvironmentReward();
                break;

            case RewardType.StoneDoor:
                OpenStoneDoor();
                break;

            case RewardType.None:
                break;
        }

        ShowSuccessMessage();
    }

    /// <summary>
    /// Gọi khi minigame thất bại
    /// </summary>
    public void OnFailure()
    {
        ShowFailureMessage();
    }

    private void GiveItemReward()
    {
        if (inventoryService != null && !string.IsNullOrEmpty(rewardItemID))
        {
            inventoryService.AddItem(rewardItemID);
            uiService?.ShowItemPickup(rewardItemID, rewardIcon);
            Debug.Log($"[MinigameReward] Item reward given: {rewardItemID}");
        }
        else
        {
            Debug.LogWarning("[MinigameReward] Cannot give item reward - inventory service or rewardItemID is null");
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
            Debug.Log($"[MinigameReward] Environment reward spawned: {rewardPrefab.name}");
        }
        else
        {
            Debug.LogWarning("[MinigameReward] Cannot spawn environment reward - rewardPrefab is null");
        }
    }

    private void OpenStoneDoor()
    {
        if (stoneDoor != null)
        {
            var player = FindObjectOfType<PlayerInteractionController>();
            if (player != null)
            {
                stoneDoor.OnInteract(player);
                Debug.Log($"[MinigameReward] Stone door opened automatically");
                uiService?.ShowMessage("Cửa đá đã mở!", 2f);
            }
            else
            {
                Debug.LogWarning("[MinigameReward] Cannot open stone door - player not found");
            }
        }
        else
        {
            Debug.LogWarning("[MinigameReward] Cannot open stone door - stoneDoor reference is null");
        }
    }

    private void ShowSuccessMessage()
    {
        if (!string.IsNullOrEmpty(successMessage))
        {
            uiService?.ShowMessage(successMessage, 2f);
        }
    }

    private void ShowFailureMessage()
    {
        if (!string.IsNullOrEmpty(failureMessage))
        {
            uiService?.ShowMessage(failureMessage, 2f);
        }
    }

    private void OnValidate()
    {
        ValidateRewardSettings();
    }
}