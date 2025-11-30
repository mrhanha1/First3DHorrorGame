using UnityEngine;

/// <summary>
/// Điểm tương tác để vào minigame
/// CHỈ quản lý: điều kiện vào game, item requirement
/// KHÔNG quản lý: logic game, reward
/// </summary>
public class MinigameInteractable : InteractableBase
{
    [Header("Minigame Reference")]
    [SerializeField] private MinigameBase minigame;

    [Header("Lock Settings")]
    [SerializeField] private bool lockAfterCompleted = false;
    [SerializeField] private string completedPromptText = "Đã giải rồi";

    [Header("Item Requirements")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemID;
    [SerializeField] private bool consumeItemOnUse = false;

    private IMinigameService minigameService;
    private IInventoryService inventoryService;
    private IUIService uiService;
    public bool isCompleted = false;
    public string minigameID;

    protected override void Awake()
    {
        base.Awake();

        minigameService = ServiceLocator.Get<IMinigameService>();
        inventoryService = ServiceLocator.Get<IInventoryService>();
        uiService = ServiceLocator.Get<IUIService>();

        if (minigame == null)
        {
            minigame = GetComponentInChildren<MinigameBase>();
            if (minigame == null)
                Debug.LogError($"[MinigameInteractable] No minigame assigned on {gameObject.name}");
        }
    }

    private void Start()
    {
        // Subscribe event để biết khi nào game hoàn thành
        if (minigame != null)
        {
            minigame.OnGameCompleted += HandleGameCompleted;
        }
    }

    public override bool CanInteract(PlayerInteractionController player)
    {
        // Đã hoàn thành và bị lock
        if (isCompleted && lockAfterCompleted)
            return false;

        // Cần item
        if (requiresItem && inventoryService != null)
        {
            return inventoryService.HasItem(requiredItemID);
        }
        if (isCompleted)
        {
            minigame.CompleteSuccess();
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
            uiService?.ShowMessage("Không thể chơi lúc này");
            return;
        }

        // Tiêu thụ item nếu cần
        if (requiresItem && consumeItemOnUse && inventoryService != null)
        {
            inventoryService.RemoveItem(requiredItemID);
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
        if (minigameService.CurrentMinigame != null)
        {
            if (minigameService.CurrentMinigame == minigame)
                minigameService.ResumeMinigame();
            else
                uiService?.ShowMessage("Đang có minigame khác đang chạy");
            return;
        }
        minigameService.StartMinigame(minigame);
    }

    private void HandleGameCompleted(bool success)
    {
        if (success)
        {
            isCompleted = true;
            Debug.Log($"[MinigameInteractable] {gameObject.name} completed successfully");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe event
        if (minigame != null)
        {
            minigame.OnGameCompleted -= HandleGameCompleted;
        }
    }
}