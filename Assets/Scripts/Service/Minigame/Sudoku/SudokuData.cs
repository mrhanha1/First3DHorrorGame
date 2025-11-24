using UnityEngine;

#region Sudoku Cell

/// <summary>
/// Data cho một ô Sudoku
/// </summary>
[System.Serializable]
public class SudokuCell
{
    public int value;           // Giá trị hiện tại (0 = empty)
    public int solution;        // Giá trị đúng
    public bool isFixed;        // Ô cố định (không thể sửa)
    public int row;             // Vị trí row
    public int col;             // Vị trí column

    public SudokuCell(int row, int col, int solution, bool isFixed)
    {
        this.row = row;
        this.col = col;
        this.solution = solution;
        this.isFixed = isFixed;
        this.value = isFixed ? solution : 0;
    }

    public bool IsEmpty => value == 0;
    public bool IsValid => value == solution;

    public void SetValue(int val)
    {
        if (isFixed) return;
        value = Mathf.Clamp(val, 0, 9);
    }

    public void Clear()
    {
        if (isFixed) return;
        value = 0;
    }

    public void IncreaseValue()
    {
        if (isFixed) return;
        value++;
        if (value > 9) value = 1;
    }

    public void DecreaseValue()
    {
        if (isFixed) return;
        value--;
        if (value < 1) value = 9;
    }
}

#endregion

#region Sudoku Grid

/// <summary>
/// Grid 9x9 với puzzle cố định
/// </summary>
public class SudokuGrid
{
    public const int GRID_SIZE = 9;
    public SudokuCell[,] cells;

    public SudokuGrid()
    {
        cells = new SudokuCell[GRID_SIZE, GRID_SIZE];
    }

    public SudokuCell GetCell(int row, int col)
    {
        if (row < 0 || row >= GRID_SIZE || col < 0 || col >= GRID_SIZE)
            return null;
        return cells[row, col];
    }

    public void SetCellValue(int row, int col, int value)
    {
        var cell = GetCell(row, col);
        cell?.SetValue(value);
    }

    public void IncreaseCell(int row, int col)
    {
        var cell = GetCell(row, col);
        cell?.IncreaseValue();
    }

    public void DecreaseCell(int row, int col)
    {
        var cell = GetCell(row, col);
        cell?.DecreaseValue();
    }

    public bool IsComplete()
    {
        for (int row = 0; row < GRID_SIZE; row++)
        {
            for (int col = 0; col < GRID_SIZE; col++)
            {
                var cell = cells[row, col];
                if (cell.IsEmpty || !cell.IsValid)
                    return false;
            }
        }
        return true;
    }

    public int GetErrorCount()
    {
        int count = 0;
        for (int row = 0; row < GRID_SIZE; row++)
        {
            for (int col = 0; col < GRID_SIZE; col++)
            {
                var cell = cells[row, col];
                if (!cell.IsEmpty && !cell.IsValid)
                    count++;
            }
        }
        return count;
    }

    public int GetFilledCount()
    {
        int count = 0;
        for (int row = 0; row < GRID_SIZE; row++)
        {
            for (int col = 0; col < GRID_SIZE; col++)
            {
                if (!cells[row, col].IsEmpty)
                    count++;
            }
        }
        return count;
    }
}

#endregion

#region Sudoku Data

/// <summary>
/// Puzzle Sudoku cố định - độ khó dễ
/// </summary>
public static class SudokuData
{
    // Solution (đáp án đúng)
    private static readonly int[,] SOLUTION = new int[,]
    {
        {5, 3, 4, 6, 7, 8, 9, 1, 2},
        {6, 7, 2, 1, 9, 5, 3, 4, 8},
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 9}
    };

    // Puzzle (0 = ô trống cần điền)
    private static readonly int[,] PUZZLE = new int[,]
    {
        //{5, 3, 0, 0, 7, 0, 0, 0, 0},
        //{6, 0, 0, 1, 9, 5, 0, 0, 0},
        //{0, 9, 8, 0, 0, 0, 0, 6, 0},
        //{8, 0, 0, 0, 6, 0, 0, 0, 3},
        //{4, 0, 0, 8, 0, 3, 0, 0, 1},
        //{7, 0, 0, 0, 2, 0, 0, 0, 6},
        //{0, 6, 0, 0, 0, 0, 2, 8, 0},
        //{0, 0, 0, 4, 1, 9, 0, 0, 5},
        //{0, 0, 0, 0, 8, 0, 0, 7, 9}
        {5, 3, 4, 6, 7, 8, 9, 1, 2},
        {6, 7, 2, 1, 9, 5, 3, 4, 8},
        {1, 9, 8, 3, 4, 2, 5, 6, 7},
        {8, 5, 9, 7, 6, 1, 4, 2, 3},
        {4, 2, 6, 8, 5, 3, 7, 9, 1},
        {7, 1, 3, 9, 2, 4, 8, 5, 6},
        {9, 6, 1, 5, 3, 7, 2, 8, 4},
        {2, 8, 7, 4, 1, 9, 6, 3, 5},
        {3, 4, 5, 2, 8, 6, 1, 7, 0}
    };

    /// <summary>
    /// Tạo grid từ puzzle cố định
    /// </summary>
    public static SudokuGrid CreateGrid()
    {
        SudokuGrid grid = new SudokuGrid();

        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                int puzzleValue = PUZZLE[row, col];
                int solutionValue = SOLUTION[row, col];
                bool isFixed = puzzleValue != 0;

                grid.cells[row, col] = new SudokuCell(row, col, solutionValue, isFixed);
            }
        }

        return grid;
    }
}

#endregion