using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SimpleBot : MonoBehaviour
{
    private CaroMinigame caroMinigame;
    private List<Cell> cells;

    public void Initialize(CaroMinigame minigame, List<Cell> boardCells)
    {
        caroMinigame = minigame;
        cells = boardCells;
    }

    public void MakeMove()
    {
        if (caroMinigame == null)
        {
            Debug.LogWarning("CaroMinigame not initialized!", this);
            return;
        }
        var (row, col) = ChooseBestMove();
        if (row >= 0 && col >= 0)
        {
            Debug.Log($"[SimpleBot] Bot placing piece at [{row}, {col}]");
            caroMinigame.MakeMove(row, col, "o");
        }
        else
        {
            Debug.LogWarning("[SimpleBot] No valid move found!");
        }
    }

    private (int row, int col) ChooseBestMove()
    {
        // 1. Try to win
        foreach (var (row, col) in caroMinigame.GetAllEmptyCells())
            if (caroMinigame.TryMoveAndCheckWin(row, col, "o"))
                return (row, col);

        // 2. Block opponent's winning move
        foreach (var (row, col) in caroMinigame.GetAllEmptyCells())
            if (caroMinigame.TryMoveAndCheckWin(row, col, "x"))
                return (row, col);

        // 3. Block opponent's 3+ consecutive threats
        var blockThreeMoves = caroMinigame.GetAllEmptyCells()
            .Where(pos => CountThreatLength(pos.row, pos.col, "x") >= 3)
            .ToList();

        if (blockThreeMoves.Count > 0)
        {
            return blockThreeMoves.OrderByDescending(pos => CountThreatLength(pos.row, pos.col, "x")).First();
        }

        // 4. Choose best scoring move
        var bestMove = caroMinigame.GetAllEmptyCells()
            .Select(pos => (pos, ScoreMove(pos.row, pos.col)))
            .OrderByDescending(x => x.Item2)
            .FirstOrDefault();

        return bestMove.pos;
    }

    private int CountThreatLength(int row, int col, string symbol)
    {
        int maxThreat = 0;

        foreach (var (di, dj) in new[] { (0, 1), (1, 0), (1, 1), (1, -1) })
        {
            int forward = CountConsecutive(row, col, di, dj, symbol, false);
            int backward = CountConsecutive(row, col, -di, -dj, symbol, false);
            int totalLength = 1 + forward + backward;

            maxThreat = Math.Max(maxThreat, totalLength);
        }

        return maxThreat;
    }

    private int CountConsecutive(int row, int col, int di, int dj, string symbol, bool includeEmpty)
    {
        int count = 0;
        for (int step = 1; step <= 4; step++)
        {
            int newRow = row + step * di;
            int newCol = col + step * dj;

            if (caroMinigame.IsOutOfBounds(newRow, newCol)) break;

            string value = caroMinigame.GetMatrixValue(newRow, newCol);
            if (value == symbol || (includeEmpty && string.IsNullOrEmpty(value)))
                count++;
            else
                break;
        }
        return count;
    }

    private int ScoreMove(int row, int col)
    {
        int score = 0;
        foreach (var (di, dj) in new[] { (0, 1), (1, 0), (1, 1), (1, -1) })
        {
            score += ScoreDirection(row, col, di, dj, "o") * 2; // Prioritize bot's moves
            score += ScoreDirection(row, col, di, dj, "x");     // Consider blocking player
        }
        return score;
    }

    private int ScoreDirection(int row, int col, int di, int dj, string symbol)
    {
        int score = 0;
        int count = 0;
        bool hasEmpty = false;

        for (int step = -4; step <= 4; step++)
        {
            if (step == 0) continue; // Skip the move position itself

            int newRow = row + step * di;
            int newCol = col + step * dj;

            if (caroMinigame.IsOutOfBounds(newRow, newCol)) continue;

            string value = caroMinigame.GetMatrixValue(newRow, newCol);
            if (value == symbol)
                count++;
            else if (string.IsNullOrEmpty(value))
                hasEmpty = true;
            else
                break;
        }

        if (hasEmpty && count > 0)
            score += count * count; // Square count for higher priority

        return score;
    }
}