using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý lifecycle, khong co camera, input của minigame
/// KHÔNG quản lý logic game hay reward
/// </summary>
public class MinigameServiceManager : MonoBehaviour, IMinigameService
{
    [Header("Main Game References")]
    [SerializeField] private string mainGameInputActionMap = "Player";
    [SerializeField] private CinemachineVirtualCamera mainGameCamera;

    [Header("Setting")]
    [SerializeField] private bool pauseGameWhileInMinigame = false;

    private IMinigame currentMinigame = null;
    private PlayerInteractionController playerController = null;
    private IInputService inputService = null;

    private string previousInputActionMap;
    private CinemachineVirtualCamera previousCamera;
    private string minigameInputMap;
    private CinemachineVirtualCamera minigameCamera;


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

    public void StartMinigame(IMinigame minigame)
    {
        if (currentMinigame != null)
        {
            Debug.LogWarning("[MinigameService] A minigame is already running");
            return;
        }
        if (minigame == null)
        {
            Debug.LogError("[MinigameService] Minigame is null");
            return;
        }

        Debug.Log($"[MinigameService] Starting minigame: {minigame.MinigameName}");

        currentMinigame = minigame;
        SaveCurrentState();

        minigameInputMap = minigame.GetInputActionMap();
        minigameCamera = minigame.GetVirtualCamera();

        LockPlayer();
        if (minigame.GetInputActionMap() != null)
            SwitchActionMap(minigame.GetInputActionMap());
        if (minigame.GetVirtualCamera() != null)
            SwitchCamera(minigame.GetVirtualCamera());

        if (pauseGameWhileInMinigame)
            Time.timeScale = 0f;

        minigame.OnEnter();
    }

    public void ExitMinigame()
    {
        if (currentMinigame == null)
        {
            Debug.LogWarning("[MinigameService] No minigame is running");
            return;
        }

        Debug.Log($"[MinigameService] Exit minigame: {currentMinigame.MinigameName}");

        RestoreState();

        if (pauseGameWhileInMinigame)
            Time.timeScale = 1f;

        currentMinigame = null;
    }
    public void PauseMinigame()
    {
        if (currentMinigame == null)
        {
            Debug.LogWarning("[MinigameService] No minigame is running to pause");
            return;
        }
        Debug.Log($"[MinigameService] Pausing minigame: {currentMinigame.MinigameName}");
        if (pauseGameWhileInMinigame)
            Time.timeScale = 1f;
        RestoreState();
    }
    public void ResumeMinigame()
    {
        if (currentMinigame == null)
        {
            Debug.LogWarning("[MinigameService] No minigame is running to resume");
            return;
        }
        Debug.Log($"[MinigameService] Resuming minigame: {currentMinigame.MinigameName}");
        LockPlayer();
        if (minigameInputMap != null)
            SwitchActionMap(minigameInputMap);
        if (minigameCamera != null)
            SwitchCamera(minigameCamera);
        currentMinigame.OnResume();

        if (pauseGameWhileInMinigame)
            Time.timeScale = 0f;
    }
    public Dictionary<string, bool> GetAllMinigameStates()
    {
        Dictionary<string, bool> states = new();
        foreach (var minigame in FindObjectsOfType<MinigameInteractable>())
        {
            states[minigame.minigameID] = minigame.isCompleted;
        }
        return states;
    }
    public void SetMinigameStates(Dictionary<string, bool> states)
    {
        foreach (var minigame in FindObjectsOfType<MinigameInteractable>())
        {
            if (states.ContainsKey(minigame.minigameID))
            {
                minigame.isCompleted = states[minigame.minigameID];
            }
        }
    }

    private void SaveCurrentState()
    {
        previousInputActionMap = mainGameInputActionMap;
        previousCamera = mainGameCamera;
        wasPlayerLocked = playerController != null && !playerController.enabled;
    }

    private void RestoreState()
    {
        SwitchActionMap(previousInputActionMap);
        SwitchCamera(previousCamera);
        if (!wasPlayerLocked)
        {
            UnlockPlayer();
        }
    }

    private void SwitchCamera(CinemachineVirtualCamera targetCam)
    {
        if (targetCam == null)
        {
            Debug.LogWarning("[MinigameService] switch camera is null");
            return;
        }
        if (targetCam == minigameCamera)
            minigameCamera.Priority = 20;
        else
            minigameCamera.Priority = 1;
        Debug.Log($"[MinigameService] Switch camera to {targetCam.name}");
    }

    private void SwitchActionMap(string actionMapName)
    {
        if (string.IsNullOrEmpty(actionMapName))
        {
            Debug.LogWarning("[MinigameService] action map is null or empty");
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
            ExitMinigame();
        }
    }
}