using Cinemachine;
using System;
using UnityEngine;

public class MinigameServiceManager : MonoBehaviour, IMinigameService
{
    [Header("Main Game References")]
    [SerializeField] private CinemachineVirtualCamera mainGameCam;
    [SerializeField] private string mainGameInputActionMap = "Player";

    [Header("Setting")]
    [SerializeField] private bool pauseGameWhileInMinigame = true;

    private IMinigame currentMinigame = null;
    private Action onMinigameComplete = null;
    private PlayerInteractionController playerController = null;
    private IInputService inputService = null;

    private CinemachineVirtualCamera previousCamera;
    private string previousInputActionMap;
    private bool wasPlayerLocked = false;

    public IMinigame CurrentMinigame => currentMinigame;
    public bool IsMinigameActive => currentMinigame != null;

    private void Start()
    {
        inputService = ServiceLocator.Get<IInputService>();
        playerController = FindObjectOfType<PlayerInteractionController>();
    }
    private void Update()
    {
        if (currentMinigame != null)
        {
            currentMinigame.OnUpdate();
        }
    }
    public void StartMinigame(IMinigame minigame, Action onComplete = null)
    {
        if (currentMinigame != null)
        {
            Debug.LogWarning("[MinigameService] A minigame is already running. Cannot start a new one");
            return;
        }
        if (minigame == null)
        {
            Debug.LogError("[MinigameService] Minigame is null");
            return;
        }

        Debug.Log($"[MinigameService] Starting minigame: {minigame.MinigameName}");

        currentMinigame = minigame;
        onMinigameComplete = onComplete;
        SaveCurrentState();
        LockPlayer();
        SwitchCamera(minigame.GetVirtualCamera());
        SwitchActionMap(minigame.GetInputActionMap());

        if (pauseGameWhileInMinigame)
        {
            Time.timeScale = 0f;
        }
        minigame.OnEnter();
    }
    public void ExitMinigame (bool success = false)
    {
        if (currentMinigame == null)
        {
            Debug.LogWarning("[MinigameService] No minigame is running");
            return;
        }
        Debug.Log($"[MinigameService] Exiting minigame: {currentMinigame.MinigameName} - Success: {success}");
        currentMinigame.OnExit(success);
        RestoreState();

        if (pauseGameWhileInMinigame)
        {
            Time.timeScale = 1f;
        }
        onMinigameComplete?.Invoke();
        onMinigameComplete = null;
        currentMinigame = null;
    }
    private void SaveCurrentState()
    {
        previousCamera = mainGameCam;
        previousInputActionMap = mainGameInputActionMap;
        wasPlayerLocked = playerController != null && playerController.enabled == false;
    }
    private void RestoreState()
    {
        SwitchCamera(previousCamera);
        SwitchActionMap(previousInputActionMap);
        if (!wasPlayerLocked)
        {
            UnlockPlayer();
        }
    }
    private void SwitchCamera(CinemachineVirtualCamera targetCam)
    {
        if (targetCam == null)
        {
            Debug.LogWarning("[MinigameService] target camera is null. Cannot switch");
            return;
        }
        Debug.Log("[MinigameService] Switch camera to " + targetCam.name);
    }
    private void SwitchActionMap(string actionMapName)
    {
        if (string.IsNullOrEmpty(actionMapName))
        {
            Debug.LogWarning("[MinigameService] action map is null or empty. Cannot switch");
            return;
        }
        if (inputService is InputSystemService inputSys)
        {
            inputSys.SwitchActionMap(actionMapName);
            Debug.Log($"[MinigameService] Switch action map to {actionMapName}");
        }
    }
    private void LockPlayer()
    {
        if (playerController != null)
        {
            playerController.LockInteraction();
            playerController.LockMovement();
        }
    }
    private void UnlockPlayer()
    {
        if (playerController != null)
        {
            playerController.UnlockInteraction();
            playerController.UnlockMovement();
        }
    }
    private void OnDestroy()
    {
        if (currentMinigame != null)
        {
            ExitMinigame(false);
        }
    }
}