using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private IGameStateService gameState;
    private IInputService inputService;

    void Start()
    {
        gameState = ServiceLocator.Get<IGameStateService>();
        inputService = ServiceLocator.Get<IInputService>();
        pauseMenuUI.SetActive(false);
        if (gameState == null || inputService == null)
        {
            Debug.LogError("[PauseManager] Required services not found!");
        }
    }

    void Update()
    {
        HandlePauseInput();
    }
    public void HandlePauseInput()
    {
        if (inputService.IsCancelPressed)
        {
            if (gameState.IsGamePaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameState.PauseGame();
        //Debug.Log("Game Paused");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        gameState.ResumeGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}