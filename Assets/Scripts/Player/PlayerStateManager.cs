using UnityEngine;
using StarterAssets;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private FirstPersonController movementController;
    [SerializeField] private PlayerInteractionController interactionController;

    [Header("State Flags")]
    [SerializeField] private bool isMovementLocked = false;
    [SerializeField] private bool isInteractionLocked = false;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = false;

    public bool IsMovementLocked => isMovementLocked;
    public bool IsInteractionLocked => isInteractionLocked;
    private void Awake()
    {
        InitializeComponents();
    }
    private void Start()
    {
        var gameState = ServiceLocator.Get<IGameStateService>();
        if (gameState is GameStateService stateService)
        {
            stateService.Initialize(this);
        }
    }
    private void InitializeComponents()
    {
        movementController = GetComponent<FirstPersonController>();
        interactionController = GetComponent<PlayerInteractionController>();

        if (movementController == null)
        {
            Debug.LogError("[PlayerStateManager] FirstPersonController not found!");
        }

        if (interactionController == null)
        {
            Debug.LogError("[PlayerStateManager] PlayerInteractionController not found!");
        }

        if (enableDebug)
        {
            Debug.Log("[PlayerStateManager] Initialized successfully");
        }
    }
    public void LockMovement(bool locked)
    {
        if (isMovementLocked) return;
        isMovementLocked = locked;
        if (movementController != null)
        {
            movementController.enabled = !locked;
        }
        if (enableDebug)
        {
            Debug.Log("[PlayerStateManager] Movement locked");
        }
    }
    public void LockInteraction( bool locked)
    {
        if (isInteractionLocked) return;

        isInteractionLocked = locked;

        if (interactionController != null)
        {
            interactionController.LockInteraction();
        }

        if (enableDebug)
        {
            Debug.Log("[PlayerStateManager] Interaction locked");
        }
    }
    #region Utility Methods
    /// <summary>
    /// Kiểm tra xem nhân vật có thể di chuyển không
    /// </summary>
    public bool CanMove()
    {
        return !isMovementLocked && movementController != null && movementController.enabled;
    }

    /// <summary>
    /// Kiểm tra xem nhân vật có thể tương tác không
    /// </summary>
    public bool CanInteract()
    {
        return !isInteractionLocked && interactionController != null;
    }
    #endregion
}