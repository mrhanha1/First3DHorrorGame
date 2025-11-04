using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float raycastInterval = 0.1f;


    private IUIService uiService;
    private IInputService inputService;
    private ICameraProvider cameraProvider;

    private Dictionary<InteractionType, IInteractionStrategy> strategies;
    private IInteractable currentInteractable;
    private IInteractionStrategy currentStrategy;
    private float raycastTimer =0f;
    private bool isLocked = false;


    private void Awake()
    {
        uiService = ServiceLocator.Get<IUIService>();
        inputService = ServiceLocator.Get<IInputService>();
        cameraProvider = ServiceLocator.Get<ICameraProvider>();

        strategies = new Dictionary<InteractionType, IInteractionStrategy> // dict chiến lược lưu cặp {loại tương tác, chiến lược tương tác tương ứng}
        {
            {InteractionType.Instant, new InstantInteractionStrategy(KeyCode.E) },
            {InteractionType.Hold, new HoldInteractionStrategy(KeyCode.E) },
            {InteractionType.Proximity, new ProximityInteractionStrategy() }
        };
    }
    void Update()
    {
        if (isLocked) return;
        raycastTimer += Time.deltaTime;
        if (raycastTimer >= raycastInterval) // chỉ gọi raycast sau mỗi khoảng thời gian raycastInterval
        {
            DetectInteractable();
            raycastTimer = 0f;
        }
        currentStrategy?.HandleInput(currentInteractable, this);
    }
    private void DetectInteractable()
    {
        if (!cameraProvider.IsValid()) return;

        Ray ray = new Ray(cameraProvider.GetCameraPosition(), cameraProvider.GetCameraForward());

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                SetCurrentInteractable(interactable);
            }
        }
        SetCurrentInteractable(null);
    }
    private void SetCurrentInteractable(IInteractable interactable)
    { 
        if (currentInteractable == interactable) return;
        if (currentInteractable != null)
        {
            if (currentInteractable is ILookable lookable)
                lookable.OnLookAway(this);
            currentStrategy?.Reset();
        }
        currentInteractable = interactable;
        if (currentInteractable != null)
        {
            if (currentInteractable is ILookable lookable)
                lookable.OnLookAt(this);
            InteractionType type = DetermineInteractionType(currentInteractable); // xác định loại tương tác
            currentStrategy = strategies[type]; // lấy chiến lược tương ứng loại tương tác từ dict

            if (currentInteractable.CanInteract(this)) // hàm này trả về true
                uiService.ShowPrompt(currentInteractable.getPromptText()); // lệnh cụ thể hiển thị prompt text
        }
        else //
        {
            uiService.HidePrompt(); // ẩn prompt
            currentStrategy = null;
        }
    }
    private InteractionType DetermineInteractionType(IInteractable interactable) // hàm if else trả về 1 trong 3 loại tương tác
    {
        if (interactable is IProximityTriggerable) return InteractionType.Proximity;
        if (interactable is IHoldable) return InteractionType.Hold;
        return InteractionType.Instant;
    }
    public void LockInteraction() //hàm dùng cho trường hợp khoá mọi tương tác của player
    {
        isLocked = true;
        SetCurrentInteractable(null);
    }
    public void UnlockInteraction()
    {
        isLocked = false;
    }
    public void LockMovement()
    {
        var movement = GetComponent<PlayerMovementController>();
        if (movement != null) movement.enabled = false;
    }
    public void UnlockMovement()
    {
        var movement = GetComponent<PlayerMovementController>();
        if (movement != null) movement.enabled = true;
    }
}
