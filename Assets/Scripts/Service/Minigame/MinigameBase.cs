using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class MinigameBase : MonoBehaviour, IMinigame
{
    [Header("Minigame Settings")]
    [SerializeField] protected string minigameName = "Minigame";
    [SerializeField] protected CinemachineVirtualCamera virtualCamera;
    [SerializeField] protected GameObject minigameUI;
    [SerializeField] protected string inputActionMap = "Minigame";
    [SerializeField] protected bool canExit = true;

    [Header("Audio")]
    [SerializeField] protected AudioClip enterSound;
    [SerializeField] protected AudioClip exitSound;
    [SerializeField] protected AudioClip successSound;
    [SerializeField] protected AudioClip failureSound;

    [Header("Control Buttons - Optional")]
    [SerializeField] protected Button upButton;
    [SerializeField] protected Button downButton;
    [SerializeField] protected Button leftButton;
    [SerializeField] protected Button rightButton;
    [SerializeField] protected Button increaseButton;
    [SerializeField] protected Button decreaseButton;
    [SerializeField] protected Button submitButton;
    [SerializeField] protected Button resetButton;
    [SerializeField] protected Button exitButton;

    protected IInputService inputService;
    protected IAudioService audioService;
    protected IUIService uiService;
    protected bool isActive = false;

    // Input System
    private PlayerInputActions inputActions;
    private InputActionMap minigameActionMap;

    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction increaseAction;
    private InputAction decreaseAction;
    private InputAction submitAction;
    private InputAction resetAction;
    private InputAction cancelAction;

    public string MinigameName => minigameName;
    public bool CanExit => canExit;

    protected virtual void Awake()
    {
        if (minigameUI) minigameUI.SetActive(false);
        if (virtualCamera) virtualCamera.Priority = 0;

        InitializeInputActions();
        SetupButtons();
    }

    protected virtual void Start()
    {
        inputService = ServiceLocator.Get<IInputService>();
        audioService = ServiceLocator.Get<IAudioService>();
        uiService = ServiceLocator.Get<IUIService>();
    }

    private void InitializeInputActions()
    {
        inputActions = new PlayerInputActions();
        minigameActionMap = inputActions.asset.FindActionMap("Minigame");

        if (minigameActionMap != null)
        {
            upAction = minigameActionMap.FindAction("Up");
            downAction = minigameActionMap.FindAction("Down");
            leftAction = minigameActionMap.FindAction("Left");
            rightAction = minigameActionMap.FindAction("Right");
            increaseAction = minigameActionMap.FindAction("Increase");
            decreaseAction = minigameActionMap.FindAction("Decrease");
            submitAction = minigameActionMap.FindAction("Submit");
            resetAction = minigameActionMap.FindAction("Reset");
            cancelAction = minigameActionMap.FindAction("Cancel");

            // Subscribe events
            if (upAction != null)
                upAction.performed += _ => OnUpPressed();

            if (downAction != null)
                downAction.performed += _ => OnDownPressed();

            if (leftAction != null)
                leftAction.performed += _ => OnLeftPressed();

            if (rightAction != null)
                rightAction.performed += _ => OnRightPressed();

            if (increaseAction != null)
                increaseAction.performed += _ => OnIncreasePressed();

            if (decreaseAction != null)
                decreaseAction.performed += _ => OnDecreasePressed();

            if (submitAction != null)
                submitAction.performed += _ => OnSubmitPressed();

            if (resetAction != null)
                resetAction.performed += _ => OnResetPressed();

            if (cancelAction != null)
                cancelAction.performed += _ => OnCancelPressed();
        }
        else
        {
            Debug.LogError("[MinigameBase] 'Minigame' Action Map not found!");
        }
    }

    private void SetupButtons()
    {
        upButton?.onClick.AddListener(() => OnUpPressed());
        downButton?.onClick.AddListener(() => OnDownPressed());
        leftButton?.onClick.AddListener(() => OnLeftPressed());
        rightButton?.onClick.AddListener(() => OnRightPressed());

        increaseButton?.onClick.AddListener(() => OnIncreasePressed());
        decreaseButton?.onClick.AddListener(() => OnDecreasePressed());

        submitButton?.onClick.AddListener(() => OnSubmitPressed());
        resetButton?.onClick.AddListener(() => OnResetPressed());
        exitButton?.onClick.AddListener(() => OnCancelPressed());
    }

    public CinemachineVirtualCamera GetVirtualCamera() => virtualCamera;
    public string GetInputActionMap() => inputActionMap;
    public GameObject GetUI() => minigameUI;

    public virtual void OnEnter()
    {
        isActive = true;
        if (minigameUI) minigameUI.SetActive(true);
        if (virtualCamera) virtualCamera.Priority = 20;

        // Enable minigame input
        if (minigameActionMap != null)
        {
            inputActions.Player.Disable();
            minigameActionMap.Enable();
        }

        if (enterSound && audioService != null)
        {
            audioService.PlaySound2D(enterSound);
        }
    }

    public virtual void OnUpdate()
    {
        // Minigame con có thể override nếu cần
    }

    public virtual void OnExit(bool success)
    {
        isActive = false;
        if (minigameUI) minigameUI.SetActive(false);
        if (virtualCamera) virtualCamera.Priority = 0;

        // Disable minigame input
        if (minigameActionMap != null)
        {
            minigameActionMap.Disable();
            inputActions.Player.Enable();
        }

        AudioClip soundToPlay = success ? (successSound ?? exitSound) : (failureSound ?? exitSound);
        if (soundToPlay && audioService != null)
        {
            audioService.PlaySound2D(soundToPlay);
        }
        Debug.Log($"[Minigame] {minigameName} exited. Success: {success}");
    }

    protected void CompleteMinigame(bool success)
    {
        IMinigameService minigameService = ServiceLocator.Get<IMinigameService>();
        minigameService?.ExitMinigame(success);
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe events
        if (upAction != null)
            upAction.performed -= _ => OnUpPressed();

        if (downAction != null)
            downAction.performed -= _ => OnDownPressed();

        if (leftAction != null)
            leftAction.performed -= _ => OnLeftPressed();

        if (rightAction != null)
            rightAction.performed -= _ => OnRightPressed();

        if (increaseAction != null)
            increaseAction.performed -= _ => OnIncreasePressed();

        if (decreaseAction != null)
            decreaseAction.performed -= _ => OnDecreasePressed();

        if (submitAction != null)
            submitAction.performed -= _ => OnSubmitPressed();

        if (resetAction != null)
            resetAction.performed -= _ => OnResetPressed();

        if (cancelAction != null)
            cancelAction.performed -= _ => OnCancelPressed();

        inputActions?.Dispose();
    }

    #region Virtual Input Methods - Override trong minigame con

    protected virtual void OnUpPressed() { }
    protected virtual void OnDownPressed() { }
    protected virtual void OnLeftPressed() { }
    protected virtual void OnRightPressed() { }
    protected virtual void OnIncreasePressed() { }
    protected virtual void OnDecreasePressed() { }
    protected virtual void OnSubmitPressed() { }
    protected virtual void OnResetPressed() { }
    protected virtual void OnCancelPressed()
    {
        // Default behavior: exit minigame
        if (canExit)
        {
            IMinigameService minigameService = ServiceLocator.Get<IMinigameService>();
            minigameService?.ExitMinigame(false);
        }
    }

    #endregion
}