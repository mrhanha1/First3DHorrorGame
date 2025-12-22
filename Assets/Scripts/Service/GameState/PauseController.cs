using UnityEngine;

public class PauseController : MonoBehaviour
{
    private IInputService inputService;
    private IGameStateService gameState;

    private void Start()
    {
        inputService = ServiceLocator.Get<IInputService>();
        gameState = ServiceLocator.Get<IGameStateService>();

        if (inputService == null || gameState == null)
        {
            Debug.LogError("[PauseController] Required services not found!");
        }
    }

    private void Update()
    {
        if (inputService != null && inputService.IsCancelPressed)
        {
            HandlePauseInput();
        }
    }

    private void HandlePauseInput()
    {
        // Không cho pause khi đang ở start menu
        if (MenuManager.Instance != null)
        {
            var currentMenuType = MenuManager.Instance.GetType()
                .GetField("currentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(MenuManager.Instance) as MenuState;

            if (currentMenuType?.MenuType == MenuType.Start)
            {
                return;
            }
        }

        if (gameState.IsGamePaused)
        {
            MenuManager.Instance?.OnBackClicked();
        }
        else
        {
            gameState.PauseGame();
            MenuManager.Instance?.OpenMenu(MenuType.Main, addToHistory: false);
        }
    }
}