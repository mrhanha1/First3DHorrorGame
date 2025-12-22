using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string gameSceneName = "GameScene";

    private GameObject currentPanel;

    private void Start()
    {
        ShowPanel(mainPanel);
        SetupMainMenuEnvironment();
    }

    private void ShowPanel(GameObject panel)
    {
        currentPanel?.SetActive(false);
        currentPanel = panel;
        currentPanel?.SetActive(true);
    }
    private void SetupMainMenuEnvironment()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (ServiceLocator.TryGet<IGameStateService>(out var gameState))
        {
            gameState.LockCursor(false);
        }
    }

    public void OnNewGameClicked()
    {
        ServiceLocator.Clear(); 
        SceneManager.LoadScene(gameSceneName);
    }
    public void OnSettingsClicked() => ShowPanel(settingsPanel);
    public void OnBackClicked() => ShowPanel(mainPanel);

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}