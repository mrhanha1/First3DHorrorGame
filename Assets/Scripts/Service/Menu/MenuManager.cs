using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject gameCompletedPanel;

    [Header("Cinemachine Cameras")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera startMenuVCam;
    [SerializeField] private int startMenuPriority = 20;
    [SerializeField] private int gamePlayPriority = 0;

    private Dictionary<MenuType, MenuState> menuStates;
    private Stack<MenuState> menuHistory;
    private MenuState currentState;
    private IGameStateService gameState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameState = ServiceLocator.Get<IGameStateService>();
        HideAllPanels();
        OpenMenu(MenuType.Start, addToHistory: false);
    }

    private void Initialize()
    {
        menuStates = new Dictionary<MenuType, MenuState>
        {
            { MenuType.Start, new StartMenuState(this, startMenuPanel) },
            { MenuType.Main, new MainMenuState(this, mainPanel) },
            { MenuType.Settings, new SettingsMenuState(this, settingsPanel) },
            { MenuType.SaveLoad, new SaveLoadMenuState(this, saveLoadPanel) },
            { MenuType.GameCompleted, new GameCompletedMenuState(this, gameCompletedPanel) }
        };

        menuHistory = new Stack<MenuState>();
    }

    public void OpenMenu(MenuType menuType, bool addToHistory = true)
    {
        if (!menuStates.ContainsKey(menuType)) return;

        MenuState newState = menuStates[menuType];

        if (currentState != null && addToHistory)
        {
            menuHistory.Push(currentState);
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void OpenSaveMenu()
    {
        if (menuStates[MenuType.SaveLoad] is SaveLoadMenuState saveLoadState)
        {
            saveLoadState.SetSaveMode(true);
        }
        OpenMenu(MenuType.SaveLoad);
    }

    public void OpenLoadMenu()
    {
        if (menuStates[MenuType.SaveLoad] is SaveLoadMenuState saveLoadState)
        {
            saveLoadState.SetSaveMode(false);
        }
        OpenMenu(MenuType.SaveLoad);
    }

    public void BackToPrevious()
    {
        if (menuHistory.Count > 0)
        {
            currentState?.Exit();
            currentState = menuHistory.Pop();
            currentState.Enter();
        }
        else
        {
            CloseAll();
        }
    }

    public void CloseAll()
    {
        currentState?.Exit();
        currentState = null;
        menuHistory.Clear();
        HideAllPanels();

        if (gameState != null)
        {
            gameState.ResumeGame();
        }
    }

    private void HideAllPanels()
    {
        startMenuPanel?.SetActive(false);
        mainPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        saveLoadPanel?.SetActive(false);
        gameCompletedPanel?.SetActive(false);
    }

    // Start Menu buttons
    public void OnStartGameClicked()
    {
        if (startMenuVCam != null)
        {
            startMenuVCam.Priority = gamePlayPriority;
        }
        CloseAll();
    }

    public void OnBackToStartMenuClicked()
    {
        if (gameState != null)
        {
            gameState.ResumeGame();
        }

        if (startMenuVCam != null)
        {
            startMenuVCam.Priority = startMenuPriority;
        }

        OpenMenu(MenuType.Start, addToHistory: false);
    }

    // In-game menu buttons
    public void OnResumeClicked() => CloseAll();
    public void OnSettingsClicked() => OpenMenu(MenuType.Settings);
    public void OnSaveClicked() => OpenSaveMenu();
    public void OnLoadClicked() => OpenLoadMenu();
    public void OnBackClicked() => BackToPrevious();

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}