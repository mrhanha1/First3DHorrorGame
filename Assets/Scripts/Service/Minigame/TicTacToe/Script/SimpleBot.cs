using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class SimpleBot : MonoBehaviour
{
    private ChessBoard chessBoard;
    private List<Cell> cells;

    public void Initialize(ChessBoard board, List<Cell> boardCells)
    {
        chessBoard = board;
        cells = boardCells;
    }

    public void MakeMove()
    {
        if (chessBoard == null) { Debug.LogWarning("ChessBoard not initialized!", this); return; }

        var (row, col) = ChooseBestMove();
        if (chessBoard.MakeMove(row, col, "o"))
        {
            var cell = cells.FirstOrDefault(c => c.Row == row && c.Column == col);
            if (cell) cell.ChangeImage("o");
            else Debug.LogError($"Cell at [{row}, {col}] not found!");
        }
    }

    private (int row, int col) ChooseBestMove()
    {
        foreach (var (row, col) in chessBoard.GetAllEmptyCells())
            if (chessBoard.TryMoveAndCheckWin(row, col, "o"))
                return (row, col);
        
        foreach (var (row, col) in chessBoard.GetAllEmptyCells())
            if (chessBoard.TryMoveAndCheckWin(row, col, "x"))
                return (row, col);

        var blockThreeMoves = chessBoard.GetAllEmptyCells()
            .Where(pos => CountThreatLength(pos.row, pos.col, "x") >= 3)
            .ToList();
    
        if (blockThreeMoves.Count > 0)
        {
            return blockThreeMoves.OrderByDescending(pos => CountThreatLength(pos.row, pos.col, "x")).First();
        }

        var bestMove = chessBoard.GetAllEmptyCells()
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
            int newRow = row + step * di, newCol = col + step * dj;
            if (chessBoard.IsOutOfBounds(newRow, newCol)) break;
        
            string value = chessBoard.GetMatrixValue(newRow, newCol);
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
            score += ScoreDirection(row, col, di, dj, "o") * 2; 
            score += ScoreDirection(row, col, di, dj, "x");     
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
            int newRow = row + step * di, newCol = col + step * dj;

            if (chessBoard.IsOutOfBounds(newRow, newCol)) continue;

            string value = chessBoard.GetMatrixValue(newRow, newCol);
            if (value == symbol) count++;
            else if (string.IsNullOrEmpty(value)) hasEmpty = true;
            else break; 
        }

        if (hasEmpty && count > 0) score += count * count; // Square count for higher priority
        return score;
    }
    
}
