using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sliding Puzzle game logic (3x3)
/// </summary>
public class SlidingPuzzleLogic
{
    private int[,] grid;
    private int[,] correctGrid;
    private int rows = 3;
    private int cols = 3;
    private int emptyRow = 2;
    private int emptyCol = 2;

    public int EmptyRow => emptyRow;
    public int EmptyCol => emptyCol;
    public int Rows => rows;
    public int Cols => cols;

    public SlidingPuzzleLogic()
    {
        grid = new int[rows, cols];
        correctGrid = new int[rows, cols];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        int pieceIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == 2 && col == 2)
                {
                    grid[row, col] = -1; // Empty cell
                    correctGrid[row, col] = -1;
                }
                else
                {
                    grid[row, col] = pieceIndex;
                    correctGrid[row, col] = pieceIndex;
                    pieceIndex++;
                }
            }
        }
    }

    public bool CanMove(int rowOffset, int colOffset, out int targetRow, out int targetCol)
    {
        targetRow = emptyRow + rowOffset;
        targetCol = emptyCol + colOffset;

        return targetRow >= 0 && targetRow < rows && targetCol >= 0 && targetCol < cols;
    }

    public bool MakeMove(int targetRow, int targetCol)
    {
        if (grid[targetRow, targetCol] == -1)
            return false;

        // Swap piece with empty cell
        grid[emptyRow, emptyCol] = grid[targetRow, targetCol];
        grid[targetRow, targetCol] = -1;

        // Update empty position
        emptyRow = targetRow;
        emptyCol = targetCol;

        return true;
    }

    public bool CheckWin()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] != correctGrid[row, col])
                    return false;
            }
        }
        return true;
    }

    public void Shuffle(int moves)
    {
        for (int i = 0; i < moves; i++)
        {
            List<(int row, int col)> validMoves = GetValidMoves();
            if (validMoves.Count > 0)
            {
                var move = validMoves[Random.Range(0, validMoves.Count)];
                MakeMove(move.row, move.col);
            }
        }
    }

    private List<(int row, int col)> GetValidMoves()
    {
        List<(int, int)> moves = new List<(int, int)>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = emptyRow + dx[i];
            int newCol = emptyCol + dy[i];

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                moves.Add((newRow, newCol));
            }
        }

        return moves;
    }

    public void Reset()
    {
        InitializeGrid();
        emptyRow = 2;
        emptyCol = 2;
    }

    public int GetPieceAt(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return -1;
        return grid[row, col];
    }
}