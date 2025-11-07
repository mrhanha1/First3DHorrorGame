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

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool showRaycastGizmos = true;
    [SerializeField] private Color rayHitColor = Color.green;
    [SerializeField] private Color rayMissColor = Color.red;

    private Vector3 lastRayOrigin;
    private Vector3 lastRayDirection;
    private bool lastRaycastHit;
    private float lastRaycastDistance;
    private Vector3 lastHitPoint;
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
        if (!cameraProvider.IsValid())
        {
            if (enableDebug) Debug.LogWarning("[PlayerInteraction] Camera provider is invalid!");
            return;
        }

        Ray ray = new Ray(cameraProvider.GetCameraPosition(), cameraProvider.GetCameraForward());

        lastRayOrigin = cameraProvider.GetCameraPosition();//
        lastRayDirection = cameraProvider.GetCameraForward();//

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            lastRaycastHit = true;//
            lastRaycastDistance = hit.distance;//
            lastHitPoint = hit.point;//

            if (enableDebug)//
            {//
                Debug.Log($"[PlayerInteraction] Raycast HIT: {hit.collider.name} at distance {hit.distance:F2}");
            }//
            //

            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if (enableDebug) Debug.Log($"[PlayerInteraction] Found IInteractable on {hit.collider.name}");
                SetCurrentInteractable(interactable);
                return;
            }
        }
        else
        {
            lastRaycastHit = false;//
            lastRaycastDistance = interactionRange;//
            lastHitPoint = lastRayOrigin + lastRayDirection * interactionRange;//

            if (enableDebug && currentInteractable != null)
            {
                Debug.Log("[PlayerInteraction] Raycast MISS - No interactable detected");
            }
        }
        SetCurrentInteractable(null);
    }
    private void SetCurrentInteractable(IInteractable interactable)
    { 
        if (currentInteractable == interactable) return;
        if (currentInteractable != null) // khi có interactable hiện tại rồi
        {
            if (currentInteractable is ILookable lookable)
                lookable.OnLookAway(this);
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

    private void OnDrawGizmos()
    {
        if (!showRaycastGizmos || !Application.isPlaying) return;

        // Draw ray
        Gizmos.color = lastRaycastHit ? rayHitColor : rayMissColor;
        Gizmos.DrawLine(lastRayOrigin, lastRayOrigin + lastRayDirection * lastRaycastDistance);

        // Draw endpoint
        Gizmos.DrawWireSphere(lastHitPoint, 0.1f);

        // Draw interaction range sphere
        if (cameraProvider != null && cameraProvider.IsValid())
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(lastRayOrigin, interactionRange);
        }

        // Draw hit point
        if (lastRaycastHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(lastHitPoint, 0.15f);
        }
    }
}
