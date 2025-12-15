using UnityEngine;
using TMPro;

public class SlidingPuzzleUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;

    public void UpdateMoves(int moves)
    {
        if (movesText != null)
            movesText.text = $"Số lần trượt: {moves}";
    }

    public void UpdateTime(float time)
    {
        if (timeText != null)
            timeText.text = $"Thời gian: {time:F1}s";
    }

    public void ShowWin(int moves, float time)
    {
        if (movesText != null)
            movesText.text = $"Số lần trượt: {moves}";

        if (timeText != null)
            timeText.text = $"Hoàn thành trong: {time:F1}s ";
    }

    public void Reset()
    {
        UpdateMoves(0);
        UpdateTime(0f);
    }
}