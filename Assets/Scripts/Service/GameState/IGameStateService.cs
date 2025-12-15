using System;

public enum GameState
{
    Playing,
    Paused,
    Loading,
    InDialogue,
    InMenu,
    InMinigame,
    InCutscene,
}
public interface IGameStateService
{
    GameState CurrentGameState { get; }
    bool IsCursorLocked { get; }
    bool IsGamePaused { get; }
    bool IsMoveLocked { get; }
    bool IsInteractionLocked { get; }
    event Action <GameState, GameState> OnGameStateChanged;
    void SetState(GameState newState);
    void PauseGame();
    void ResumeGame();
    void LockCursor(bool isLocked);
    void LockMovement(bool isLocked);
    void LockInteraction(bool isLocked);
    void SetTimeScale(float timeScale);
}