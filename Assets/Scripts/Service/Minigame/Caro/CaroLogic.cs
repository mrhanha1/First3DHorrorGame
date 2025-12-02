using System.Collections.Generic;

/// <summary>
/// Caro/Gomoku game logic
/// Chỉ chứa logic thuần túy, không phụ thuộc Unity
/// </summary>
public class CaroLogic
{
    private string[,] matrix;
    private int rows;
    private int columns;
    private string currentPlayer;

    private readonly (int row, int col)[] directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

    public int Rows => rows;
    public int Columns => columns;
    public string CurrentPlayer => currentPlayer;

    public CaroLogic(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        matrix = new string[rows, columns];
        currentPlayer = "x";

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                matrix[i, j] = "";
    }

    public bool MakeMove(int row, int col)
    {
        if (IsOutOfBounds(row, col) || !IsCellEmpty(row, col))
            return false;

        matrix[row, col] = currentPlayer;
        return true;
    }

    public void SwitchPlayer()
    {
        currentPlayer = currentPlayer == "x" ? "o" : "x";
    }

    public bool CheckWin(int row, int col)
    {
        string value = GetCell(row, col);
        if (string.IsNullOrEmpty(value)) return false;

        foreach (var (di, dj) in directions)
        {
            int count = 1 + CountDirection(row, col, di, dj, value)
                          + CountDirection(row, col, -di, -dj, value);
            if (count >= 5) return true;
        }

        return false;
    }

    private int CountDirection(int row, int col, int di, int dj, string value)
    {
        int count = 0;
        for (int step = 1; step <= 4; step++)
        {
            int newRow = row + step * di;
            int newCol = col + step * dj;

            if (IsOutOfBounds(newRow, newCol) || GetCell(newRow, newCol) != value)
                break;

            count++;
        }
        return count;
    }

    public bool TryMoveAndCheckWin(int row, int col, string symbol)
    {
        if (IsOutOfBounds(row, col) || !IsCellEmpty(row, col))
            return false;

        matrix[row, col] = symbol;
        bool isWin = CheckWin(row, col);
        matrix[row, col] = "";

        return isWin;
    }

    public string GetCell(int row, int col)
    {
        if (IsOutOfBounds(row, col)) return null;
        return matrix[row, col];
    }

    public bool IsCellEmpty(int row, int col)
    {
        return string.IsNullOrEmpty(GetCell(row, col));
    }

    public bool IsOutOfBounds(int row, int col)
    {
        return row < 0 || row >= rows || col < 0 || col >= columns;
    }

    public List<(int row, int col)> GetAllEmptyCells()
    {
        List<(int, int)> emptyCells = new ();

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                if (IsCellEmpty(i, j))
                    emptyCells.Add((i, j));

        return emptyCells;
    }

    public bool IsDraw()
    {
        return GetAllEmptyCells().Count == 0;
    }
}