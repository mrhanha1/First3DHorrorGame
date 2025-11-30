using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ChessBoard : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform board;
    public GridLayoutGroup gridLayout;
    public int columns = 15, rows = 15;
    public string CurrentPlayer { get; set; } = "x";
    
    private string[,] matrix;
    private readonly (int row, int col)[] directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

    void Start()
    {
        matrix = new string[rows, columns];
        if (gridLayout) gridLayout.constraintCount = columns;
        CreateBoard();
    }

    void CreateBoard()
    {
        if (!board || !cellPrefab) { Debug.LogError("Board or CellPrefab missing!", this); return; }

        foreach (Transform child in board) Destroy(child.gameObject);

        List<Cell> cells = new();
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < columns; j++)
        {
            var cellObj = Instantiate(cellPrefab, board);
            if (cellObj.TryGetComponent<Cell>(out var cell))
            {
                cell.Row = i; cell.Column = j; cell.Initialize(this);
                cells.Add(cell);
                matrix[i, j] = "";
            }
            else Debug.LogError($"No Cell component at [{i}, {j}]!", cellObj);
        }

        var bot = FindFirstObjectByType<SimpleBot>();
        if (bot) bot.Initialize(this, cells);
        else Debug.LogWarning("SimpleBot not found!", this);
    }

    public bool MakeMove(int row, int col, string player)
    {
        if (IsOutOfBounds(row, col) || !IsCellEmpty(row, col)) return false;

        matrix[row, col] = player;
        if (CheckWin(row, col))
        {
            Debug.Log($"Player {player} wins!");
            DisableAllCells();
            return true;
        }

        CurrentPlayer = CurrentPlayer == "x" ? "o" : "x";
        Debug.Log($"Next turn: {CurrentPlayer}");
        if (CurrentPlayer == "o") StartCoroutine(TriggerBotMove());
        return true;
    }

    IEnumerator TriggerBotMove()
    {
        yield return new WaitForSeconds(0.5f);
        var bot = FindFirstObjectByType<SimpleBot>();
        if (bot) bot.MakeMove();
        else Debug.LogWarning("SimpleBot not found!", this);
    }

    public bool CheckWin(int row, int col)
    {
        string value = matrix[row, col];
        if (string.IsNullOrEmpty(value)) return false;

        foreach (var (di, dj) in directions)
            if (1 + CountDirection(row, col, di, dj, value) + CountDirection(row, col, -di, -dj, value) >= 5)
                return true;
        return false;
    }

    int CountDirection(int row, int col, int di, int dj, string value)
    {
        int count = 0;
        for (int step = 1; step <= 4; step++)
        {
            int newRow = row + step * di, newCol = col + step * dj;
            if (IsOutOfBounds(newRow, newCol) || matrix[newRow, newCol] != value) break;
            count++;
        }
        return count;
    }

    public bool TryMoveAndCheckWin(int row, int col, string symbol)
    {
        if (IsOutOfBounds(row, col) || !IsCellEmpty(row, col)) return false;
        matrix[row, col] = symbol;
        bool isWin = CheckWin(row, col);
        matrix[row, col] = "";
        return isWin;
    }

    public string GetMatrixValue(int row, int col) => IsOutOfBounds(row, col) ? null : matrix[row, col];
    public bool IsCellEmpty(int row, int col) => string.IsNullOrEmpty(GetMatrixValue(row, col));
    public bool IsOutOfBounds(int row, int col) => row < 0 || row >= rows || col < 0 || col >= columns;

    public List<(int row, int col)> GetAllEmptyCells()
    {
        List<(int, int)> emptyCells = new();
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < columns; j++)
            if (IsCellEmpty(i, j)) emptyCells.Add((i, j));
        return emptyCells;
    }

    void DisableAllCells()
    {
        foreach (Transform child in board)
            if (child.TryGetComponent<Button>(out var button))
                button.interactable = false;
    }
}
