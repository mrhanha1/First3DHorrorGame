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

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool showRaycastGizmos = true;
    [SerializeField] private Color rayHitColor = Color.green;
    [SerializeField] private Color rayMissColor = Color.red;
    private RaycastDetector raycastDetector;
    private RaycastResult lastRaycastResult;

    private IUIService uiService;
    private IInputService inputService;
    private ICameraProvider cameraProvider;

    private Dictionary<InteractionType, IInteractionStrategy> strategies;
    private IInteractable currentInteractable;
    private IInteractionStrategy currentStrategy;

    private float raycastTimer = 0f;
    private bool isLocked = false;

    private void Awake()
    {
        uiService = ServiceLocator.Get<IUIService>();
        inputService = ServiceLocator.Get<IInputService>();
        cameraProvider = ServiceLocator.Get<ICameraProvider>();

        strategies = new Dictionary<InteractionType, IInteractionStrategy> // dict chiến lược lưu cặp {loại tương tác, chiến lược tương tác tương ứng}
        {
            {InteractionType.Instant, new InstantInteractionStrategy() },
            {InteractionType.Hold, new HoldInteractionStrategy() },
            {InteractionType.Proximity, new ProximityInteractionStrategy() }
        };

        raycastDetector = new RaycastDetector(
            cameraProvider,
            interactableLayer,
            interactionRange,
            enableDebug);
        inputService.SetCursorState(true);
        if (enableDebug)
        {
            Debug.Log($"[PlayerInteraction] Initialized - Range: {interactionRange}, Layer: {interactableLayer.value}");
        }
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
        currentStrategy?.HandleInput(currentInteractable, this, inputService);
    }
    private void DetectInteractable()
    {
        lastRaycastResult = raycastDetector.DetectInteractable();
        SetCurrentInteractable(lastRaycastResult.Interactable);
    }
    private void SetCurrentInteractable(IInteractable interactable)
    { 
        if (currentInteractable == interactable) return;
        if (currentInteractable != null) // khi có interactable hiện tại rồi
        {
            if (currentInteractable is MonoBehaviour mb && mb != null)
            {
                if (currentInteractable is ILookable lookable)
                    lookable.OnLookAway(this);
            }
            currentStrategy?.Reset();
        }
        currentInteractable = interactable;

        if (currentInteractable != null) // khi phát hiện được interactable mới
        {
            if (currentInteractable is ILookable lookable)
                lookable.OnLookAt(this);
            InteractionType type = GetInteractionType(currentInteractable); // xác định loại tương tác
            currentStrategy = strategies[type]; // lấy chiến lược tương ứng loại tương tác từ dict

            if (currentInteractable.CanInteract(this)) // hàm này trả về true
                uiService.ShowPrompt(currentInteractable.getPromptText()); // lệnh cụ thể hiển thị prompt text
        }
        else // khi k có interactable nào
        {
            uiService.HidePrompt(); // ẩn prompt
            currentStrategy = null;
        }
    }
    private InteractionType GetInteractionType(IInteractable interactable) // hàm if else trả về 1 trong 3 loại tương tác
    {
        if (interactable is IProximityTriggerable) return InteractionType.Proximity;
        if (interactable is IHoldable) return InteractionType.Hold;
        return InteractionType.Instant;
    }
    public void LockInteraction()
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
        var movement = GetComponent<FirstPersonController>();
        if (movement != null) movement.enabled = false;
    }
    public void UnlockMovement()
    {
        var movement = GetComponent<FirstPersonController>();
        if (movement != null) movement.enabled = true;
    }

    private void OnDrawGizmos()
    {
        if (!showRaycastGizmos || !Application.isPlaying) return;

        Gizmos.color = lastRaycastResult.HasHit ? rayHitColor : rayMissColor;
        Gizmos.DrawLine(
            lastRaycastResult.RayOrigin,
            lastRaycastResult.RayOrigin + lastRaycastResult.RayDirection * lastRaycastResult.RayDistance
            );
        Gizmos.DrawWireSphere(lastRaycastResult.Endpoint, 0.1f);
        if (cameraProvider != null && cameraProvider.IsValid())
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(lastRaycastResult.RayOrigin, interactionRange);
        }
        if (lastRaycastResult.HasHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastRaycastResult.Endpoint, 0.2f);
        }
    }
    private void OnValidate()
    {
        if (Application.isPlaying && raycastDetector != null)
        {
            raycastDetector.SetDebugMode(enableDebug);
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        //inputService.SetCursorState(focus && !isLocked);
    }
}
