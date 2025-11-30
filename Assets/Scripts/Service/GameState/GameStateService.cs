using StarterAssets;
using System;
using UnityEngine;

public class GameStateService : IGameStateService
{
    public GameState CurrentGameState { get; private set; } = GameState.Loading;
    public bool IsCursorLocked { get; private set; } = true;
    public bool IsGamePaused { get; private set; } = false;
    public bool IsMoveLocked { get; private set; } = false;
    public bool IsInteractionLocked { get; private set; } = false;
    public event Action<GameState, GameState> OnGameStateChanged;

    private PlayerStateManager playerStateManager;
    public void Initialize(PlayerStateManager stateManager)
    {
        this.playerStateManager = stateManager;
    }
    public void SetState(GameState newState)
    {
        if (newState != CurrentGameState)
        {
            var previousState = CurrentGameState;
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(previousState, newState);
            HandleStateChange(newState);
        }
    }
    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                LockCursor(true);
                LockMovement(false);
                LockInteraction(false);
                SetTimeScale(1f);
                break;
            case GameState.Paused:
                LockCursor(false);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(0f);
                break;
            case GameState.InMenu:
                LockCursor(false);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(0f);// tam thoi giong paused
                break;
            case GameState.InDialogue:
                LockCursor(true);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(1f);
                break;
            case GameState.InMinigame:
                LockCursor(true);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(1f);
                break;
            case GameState.InCutscene:
                LockCursor(true);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(1f);
                break;
            case GameState.Loading:
                LockCursor(true);
                LockMovement(true);
                LockInteraction(true);
                SetTimeScale(0f);
                break;
            default:
                break;
        }
    }
    public void PauseGame()
    {
        IsGamePaused = true;
        SetState(GameState.Paused);
    }
    public void ResumeGame()
    {
        IsGamePaused = false;
        SetState(GameState.Playing);
    }
    public void LockCursor(bool isLocked)
    {
        IsCursorLocked = isLocked;
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
    public void LockMovement(bool isLocked)
    {
        IsMoveLocked = isLocked;
        if (playerStateManager != null)
        {
            playerStateManager.LockMovement(isLocked);
        }
    }
    public void LockInteraction(bool isLocked)
    {
        IsInteractionLocked = isLocked;
        if (playerStateManager != null)
        {
            playerStateManager.LockInteraction(isLocked);
        }
    }
    public void SetTimeScale(float timeScale)
    {
        UnityEngine.Time.timeScale = timeScale;
    }
    public void ResetTimeScale()
    {
        UnityEngine.Time.timeScale = 1f;
    }
}