using DG.Tweening;
using UnityEngine;

/// <summary>
/// Xử lý input cho Caro game
/// Tương tự như Input Overrides trong SudokuMinigame
/// </summary>
public class CaroInputHandler
{
    private CaroMinigame caroMinigame;

    public void Initialize(CaroMinigame minigame)
    {
        this.caroMinigame = minigame;
    }

    public void OnUpPressed()
    {
        caroMinigame?.MoveSelection(0, 1);
    }

    public void OnDownPressed()
    {
        caroMinigame?.MoveSelection(0, -1);
    }

    public void OnLeftPressed()
    {
        caroMinigame?.MoveSelection(-1, 0);
    }

    public void OnRightPressed()
    {
        caroMinigame?.MoveSelection(1, 0);
    }

    public void OnIncreasePressed()
    {
        caroMinigame?.PlacePiece();
    }

    public void OnDecreasePressed()
    {
        caroMinigame?.PlacePiece();
    }

    public void OnCancelPressed()
    {
        caroMinigame?.TogglePause();
    }

    public void OnSubmitPressed()
    {
        caroMinigame?.ExitGame();
    }
}