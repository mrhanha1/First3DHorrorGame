using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SudokuMinigame : MinigameBase
{
    [Header("UI References")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private RectTransform gridContainer;

    [Header("Info Display")]
    [SerializeField] private Text progressText;
    [SerializeField] private Text errorText;

    [Header("Feedback")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip completeSound;

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 60f;
    [SerializeField] private float spacing = 2f;

    private SudokuGrid sudokuGrid;
    private SudokuCellUI[,] cellUIs;
    private int selectedRow = 0;
    private int selectedCol = 0;
    private bool isPlaying = false;

    protected override void Start()
    {
        base.Start();

        if (winPanel) winPanel.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // Create puzzle
        sudokuGrid = SudokuData.CreateGrid();
        GenerateGridUI();

        selectedRow = 0;
        selectedCol = 0;

        StartCoroutine(InitializeSelectionAfterFrame());

        isPlaying = true;
        UpdateUI();
    }

    private IEnumerator InitializeSelectionAfterFrame()
    {
        yield return null;
        UpdateSelection();
    }

    public override void OnExit(bool success)
    {
        base.OnExit(success);
        isPlaying = false;
    }

    #region Input Overrides

    protected override void OnUpPressed() => MoveSelection(0, -1);
    protected override void OnDownPressed() => MoveSelection(0, 1);
    protected override void OnLeftPressed() => MoveSelection(-1, 0);
    protected override void OnRightPressed() => MoveSelection(1, 0);
    protected override void OnIncreasePressed() => IncreaseValue();
    protected override void OnDecreasePressed() => DecreaseValue();

    protected override void OnSubmitPressed() => CheckSolution();

    protected override void OnResetPressed() => ResetAllCells();

    protected override void OnCancelPressed() => ExitGame();

    #endregion

    #region Grid UI

    private void GenerateGridUI()
    {
        ClearGrid();

        if (gridLayout)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = SudokuGrid.GRID_SIZE;
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(spacing, spacing);
        }

        cellUIs = new SudokuCellUI[SudokuGrid.GRID_SIZE, SudokuGrid.GRID_SIZE];

        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                var cellObj = Instantiate(cellPrefab, gridContainer);
                var cellUI = cellObj.GetComponent<SudokuCellUI>();

                if (cellUI == null)
                    cellUI = cellObj.AddComponent<SudokuCellUI>();

                var cellData = sudokuGrid.GetCell(row, col);
                cellUI.Initialize(cellData);

                cellUIs[row, col] = cellUI;
            }
        }
    }

    private void ClearGrid()
    {
        if (gridContainer == null) return;

        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        cellUIs = null;
    }

    #endregion

    #region Selection & Value Control

    private void MoveSelection(int deltaCol, int deltaRow)
    {
        if (!isPlaying) return;

        // Deselect current
        if (cellUIs != null && cellUIs[selectedRow, selectedCol] != null)
        {
            cellUIs[selectedRow, selectedCol].SetSelected(false);
        }

        // Update position
        selectedCol += deltaCol;
        selectedRow += deltaRow;

        // Wrap around
        if (selectedCol < 0) selectedCol = SudokuGrid.GRID_SIZE - 1;
        if (selectedCol >= SudokuGrid.GRID_SIZE) selectedCol = 0;
        if (selectedRow < 0) selectedRow = SudokuGrid.GRID_SIZE - 1;
        if (selectedRow >= SudokuGrid.GRID_SIZE) selectedRow = 0;

        UpdateSelection();
        PlaySound2D(moveSound, 0.3f);
    }

    private void UpdateSelection()
    {
        if (cellUIs == null) return;

        // Deselect all
        for (int r = 0; r < SudokuGrid.GRID_SIZE; r++)
        {
            for (int c = 0; c < SudokuGrid.GRID_SIZE; c++)
            {
                if (cellUIs[r, c] != null)
                    cellUIs[r, c].SetSelected(false);
            }
        }

        // Select current
        if (cellUIs[selectedRow, selectedCol] != null)
        {
            cellUIs[selectedRow, selectedCol].SetSelected(true);
        }
    }

    private void IncreaseValue()
    {
        if (!isPlaying) return;

        var cell = sudokuGrid.GetCell(selectedRow, selectedCol);
        if (cell == null || cell.isFixed)
        {
            PlaySound2D(errorSound, 0.5f);
            return;
        }

        sudokuGrid.IncreaseCell(selectedRow, selectedCol);
        cellUIs[selectedRow, selectedCol]?.UpdateVisuals();

        PlaySound2D(changeSound, 0.4f);
        UpdateUI();
    }

    private void DecreaseValue()
    {
        if (!isPlaying) return;

        var cell = sudokuGrid.GetCell(selectedRow, selectedCol);
        if (cell == null || cell.isFixed)
        {
            PlaySound2D(errorSound, 0.5f);
            return;
        }

        sudokuGrid.DecreaseCell(selectedRow, selectedCol);
        cellUIs[selectedRow, selectedCol]?.UpdateVisuals();

        PlaySound2D(changeSound, 0.4f);
        UpdateUI();
    }

    private void ResetAllCells()
    {
        if (!isPlaying) return;

        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                var cell = sudokuGrid.GetCell(row, col);
                if (cell != null && !cell.isFixed)
                {
                    cell.SetValue(0);
                    cellUIs[row, col]?.UpdateVisuals();
                }
            }
        }

        PlaySound2D(changeSound, 0.6f);
        UpdateUI();

        uiService?.ShowMessage("Đã xóa tất cả", 2f);
    }

    #endregion

    #region Game Logic

    private void CheckSolution()
    {
        if (!isPlaying) return;

        if (sudokuGrid.IsComplete())
        {
            OnPuzzleComplete();
        }
        else
        {
            int errors = sudokuGrid.GetErrorCount();
            int filled = sudokuGrid.GetFilledCount();
            int total = SudokuGrid.GRID_SIZE * SudokuGrid.GRID_SIZE;

            if (errors > 0)
            {
                PlaySound2D(errorSound, 0.7f);
            }

            uiService?.ShowMessage($"Tiến độ: {filled}/{total} | Lỗi: {errors}", 3f);
        }
    }

    private void ExitGame()
    {
        CompleteMinigame(false);
    }

    private void OnPuzzleComplete()
    {
        isPlaying = false;

        PlaySound2D(completeSound, 1f);

        if (winPanel)
        {
            winPanel.SetActive(true);
        }

        StartCoroutine(CompleteAfterDelay(2f));
    }

    private IEnumerator CompleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CompleteMinigame(true);
    }

    private void UpdateUI()
    {
        if (progressText)
        {
            int filled = sudokuGrid.GetFilledCount();
            int total = SudokuGrid.GRID_SIZE * SudokuGrid.GRID_SIZE;
            progressText.text = $"Tiến độ: {filled}/{total}";
        }

        if (errorText)
        {
            int errors = sudokuGrid.GetErrorCount();
            errorText.text = $"Lỗi: {errors}";
        }
    }

    #endregion

    private void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip && audioService != null)
        {
            audioService.PlaySound2D(clip, volume);
        }
    }
}