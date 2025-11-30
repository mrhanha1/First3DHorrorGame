using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;

    [Header("References")]
    [SerializeField] private SaveMenuUI saveMenuUI;

    private IGameStateService gameState;
    private ISaveService saveService;

    private void Awake()
    {
        gameState = ServiceLocator.Get<IGameStateService>();
        saveService = ServiceLocator.Get<ISaveService>();
    }

    private void Start()
    {
        ShowMainPanel();
    }

    // Panel Navigation
    public void ShowMainPanel()
    {
        HideAllPanels();
        mainPanel?.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        HideAllPanels();
        settingsPanel?.SetActive(true);
    }

    public void ShowSavePanel()
    {
        HideAllPanels();
        saveLoadPanel?.SetActive(true);
        saveMenuUI?.ShowSaveMenu(true);
    }

    public void ShowLoadPanel()
    {
        HideAllPanels();
        saveLoadPanel?.SetActive(true);
        saveMenuUI?.ShowSaveMenu(false);
    }

    private void HideAllPanels()
    {
        mainPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        saveLoadPanel?.SetActive(false);
    }

    // Main Menu Buttons
    public void OnResumeClicked()
    {
        gameState.ResumeGame();
    }

    public void OnSaveClicked()
    {
        ShowSavePanel();
    }

    public void OnLoadClicked()
    {
        ShowLoadPanel();
    }

    public void OnSettingsClicked()
    {
        ShowSettingsPanel();
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Back buttons
    public void OnBackToMain()
    {
        ShowMainPanel();
    }
}