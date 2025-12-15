using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Base class cho tất cả minigame
/// Quản lý input, audio, UI cơ bản
/// logic chung cho cac minigame
/// </summary>
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

    //[Header("Control Buttons - Optional")]
    //[SerializeField] protected Button upButton;
    //[SerializeField] protected Button downButton;
    //[SerializeField] protected Button leftButton;
    //[SerializeField] protected Button rightButton;
    //[SerializeField] protected Button increaseButton;
    //[SerializeField] protected Button decreaseButton;
    //[SerializeField] protected Button submitButton;
    //[SerializeField] protected Button resetButton;
    //[SerializeField] protected Button exitButton;

    // Event để thông báo kết quả game
    public event Action<bool> OnGameCompleted;

    protected IInputService inputService;
    protected IAudioService audioService;
    protected IUIService uiService;
    protected MinigameReward rewardComponent;
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
        //if (virtualCamera) virtualCamera.Priority = 1;

        rewardComponent = GetComponent<MinigameReward>();

        //InitializeInputActions();
        //SetupButtons();
    }

    protected virtual void Start()
    {
        inputService = ServiceLocator.Get<IInputService>();
        audioService = ServiceLocator.Get<IAudioService>();
        uiService = ServiceLocator.Get<IUIService>();
        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        if (inputService is InputSystemService inputSys)
            inputActions = inputSys.GetInputActions();
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
                upAction.performed += OnUpPerformed;
            if (downAction != null)
                downAction.performed += OnDownPerformed;
            if (leftAction != null)
                leftAction.performed += OnLeftPerformed;
            if (rightAction != null)
                rightAction.performed += OnRightPerformed;
            if (increaseAction != null)
                increaseAction.performed += OnIncreasePerformed;
            if (decreaseAction != null)
                decreaseAction.performed += OnDecreasePerformed;
            if (submitAction != null)
                submitAction.performed += OnSubmitPerformed;
            if (resetAction != null)
                resetAction.performed += OnResetPerformed;
            if (cancelAction != null)
                cancelAction.performed += OnCancelPerformed;
        }
        else
        {
            Debug.LogError("[MinigameBase] 'Minigame' Action Map not found!");
        }
    }

    //private void SetupButtons()
    //{
    //    upButton?.onClick.AddListener(() => OnUpPressed());
    //    downButton?.onClick.AddListener(() => OnDownPressed());
    //    leftButton?.onClick.AddListener(() => OnLeftPressed());
    //    rightButton?.onClick.AddListener(() => OnRightPressed());
    //    increaseButton?.onClick.AddListener(() => OnIncreasePressed());
    //    decreaseButton?.onClick.AddListener(() => OnDecreasePressed());
    //    submitButton?.onClick.AddListener(() => OnSubmitPressed());
    //    resetButton?.onClick.AddListener(() => OnResetPressed());
    //    exitButton?.onClick.AddListener(() => OnCancelPressed());
    //}

    public CinemachineVirtualCamera GetVirtualCamera() => virtualCamera;
    public string GetInputActionMap() => inputActionMap;
    public GameObject GetUI() => minigameUI;

    public virtual void OnEnter()
    {
        if (isActive)
        {
            //OnResume();
            return;
        }
        isActive = true;
        if (minigameUI) minigameUI.SetActive(true);
        //if (virtualCamera) virtualCamera.Priority = 20;


        if (enterSound && audioService != null)
        {
            audioService.PlaySound2D(enterSound);
        }
    }

    public virtual void OnUpdate()
    {
        
    }

    /// <summary>
    /// Cleanup khi thoát minigame
    /// Chỉ xử lý UI, input, camera - KHÔNG có logic business
    /// </summary>
    public virtual void OnExit()
    {
        isActive = false;
        if (minigameUI) minigameUI.SetActive(false);


        // Play exit sound
        if (exitSound && audioService != null)
        {
            audioService.PlaySound2D(exitSound);
        }
        OnGameCompleted?.Invoke(false);

        // cleanup
        IMinigameService minigameService = ServiceLocator.Get<IMinigameService>();
        minigameService?.ExitMinigame();
    }
    public virtual void OnPause()
    {
        if (minigameUI) minigameUI.SetActive(false);
        IMinigameService minigameService = ServiceLocator.Get<IMinigameService>();
        minigameService?.PauseMinigame();
    }
    public virtual void OnResume()
    {
        if (minigameUI) minigameUI.SetActive(true);
        Debug.Log("[MinigameBase] Resumed minigame");
    }

    /// <summary>
    /// Gọi khi hoàn thành thành công minigame
    /// Trigger event, give reward, rồi mới exit
    /// </summary>
    public void CompleteSuccess()
    {
        // Play success sound
        if (successSound && audioService != null)
        {
            audioService.PlaySound2D(successSound);
        }

        // Trigger event cho external listeners
        OnGameCompleted?.Invoke(true);

        // Give reward nếu có
        if (rewardComponent != null)
        {
            Component[] components = GetComponents<MinigameReward>();
            foreach (var comp in components)
            {
                if (comp is MinigameReward reward)
                {
                    reward.GiveReward();
                }
            }
        }
        uiService?.ShowMessage("Làm được rồi", 2f);

    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe events
        if (upAction != null) upAction.performed -= OnUpPerformed;
        if (downAction != null) downAction.performed -= OnDownPerformed;
        if (leftAction != null) leftAction.performed -= OnLeftPerformed;
        if (rightAction != null) rightAction.performed -= OnRightPerformed;
        if (increaseAction != null) increaseAction.performed -= OnIncreasePerformed;
        if (decreaseAction != null) decreaseAction.performed -= OnDecreasePerformed;
        if (submitAction != null) submitAction.performed -= OnSubmitPerformed;
        if (resetAction != null) resetAction.performed -= OnResetPerformed;
        if (cancelAction != null) cancelAction.performed -= OnCancelPerformed;

        inputActions?.Dispose();
    }

    #region Input Event Handlers

    private void OnUpPerformed(InputAction.CallbackContext context) => OnUpPressed();
    private void OnDownPerformed(InputAction.CallbackContext context) => OnDownPressed();
    private void OnLeftPerformed(InputAction.CallbackContext context) => OnLeftPressed();
    private void OnRightPerformed(InputAction.CallbackContext context) => OnRightPressed();
    private void OnIncreasePerformed(InputAction.CallbackContext context) => OnIncreasePressed();
    private void OnDecreasePerformed(InputAction.CallbackContext context) => OnDecreasePressed();
    private void OnSubmitPerformed(InputAction.CallbackContext context) => OnSubmitPressed();
    private void OnResetPerformed(InputAction.CallbackContext context) => OnResetPressed();
    private void OnCancelPerformed(InputAction.CallbackContext context) => OnCancelPressed();

    #endregion

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
        if (!isActive) return;
        if (canExit)
        {
            OnExit();
        }
    }

    #endregion
}