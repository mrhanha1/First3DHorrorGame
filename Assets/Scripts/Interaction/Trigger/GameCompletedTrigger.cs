using UnityEngine;

public class GameCompletedTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private IGameStateService gameState;

    private void Start()
    {
        gameState = ServiceLocator.Get<IGameStateService>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            HandleGameCompleted();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            HandleGameCompleted();
        }
    }

    private void HandleGameCompleted()
    {
        gameState?.PauseGame();
        MenuManager.Instance?.OpenMenu(MenuType.GameCompleted, addToHistory: false);
    }
}