using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Caro/Gomoku 15x15 minigame
/// Quản lý UI và game flow
/// </summary>
public class CaroMinigame : MinigameBase
{
    [Header("UI References")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private Transform boardContainer;

    [Header("Feedback")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip drawSound;

    [Header("Grid Settings")]
    [SerializeField] private int columns = 15;
    [SerializeField] private int rows = 15;
    [SerializeField] private float cellSize = 40f;
    [SerializeField] private float spacing = 2f;

    [Header("Bot Settings")]
    [SerializeField] private SimpleBot botPlayer;

    private CaroLogic caroLogic;
    private Cell[,] cellUIs;
    private CaroInputHandler inputHandler;
    private int selectedRow = 0;
    private int selectedCol = 0;
    private bool isPlaying = false;
    private bool isPaused = false;

    protected override void Start()
    {
        base.Start();
        if (winPanel) winPanel.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        caroLogic = new CaroLogic(rows, columns);
        GenerateBoardUI();

        selectedRow = 0;
        selectedCol = 0;
        isPlaying = true;
        isPaused = false;

        inputHandler = new CaroInputHandler();
        inputHandler.Initialize(this);

        StartCoroutine(InitializeSelectionAfterFrame());
    }

    private IEnumerator InitializeSelectionAfterFrame()
    {
        yield return null;
        UpdateSelection();
    }

    public override void OnExit()
    {
        base.OnExit();
        isPlaying = false;
        isPaused = false;
    }

    public override void OnPause()
    {
        base.OnPause();
        isPaused = true;
    }

    public override void OnResume()
    {
        base.OnResume();
        isPaused = false;
    }

    #region Input Overrides

    protected override void OnUpPressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnUpPressed();
    }

    protected override void OnDownPressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnDownPressed();
    }

    protected override void OnLeftPressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnLeftPressed();
    }

    protected override void OnRightPressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnRightPressed();
    }

    protected override void OnIncreasePressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnIncreasePressed();
    }

    protected override void OnDecreasePressed()
    {
        if (!isPlaying || isPaused) return;
        inputHandler?.OnDecreasePressed();
    }

    protected override void OnCancelPressed()
    {
        if (!isPlaying) return;
        inputHandler?.OnCancelPressed();
    }

    protected override void OnSubmitPressed()
    {
        if (!isPlaying) return;
        inputHandler?.OnSubmitPressed();
    }

    #endregion

    #region Grid UI

    private void GenerateBoardUI()
    {
        ClearBoard();

        if (gridLayout)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(spacing, spacing);
        }

        cellUIs = new Cell[rows, columns];
        List<Cell> cells = new List<Cell>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var cellObj = Instantiate(cellPrefab, boardContainer);
                var cell = cellObj.GetComponent<Cell>();

                if (cell == null)
                    cell = cellObj.AddComponent<Cell>();

                cell.Row = i;
                cell.Column = j;
                cell.Initialize(this);

                cellUIs[i, j] = cell;
                cells.Add(cell);
            }
        }

        if (botPlayer)
            botPlayer.Initialize(this, cells);
    }

    private void ClearBoard()
    {
        if (boardContainer == null) return;

        foreach (Transform child in boardContainer)
            Destroy(child.gameObject);

        cellUIs = null;
    }

    #endregion

    #region Selection & Move Control

    public void MoveSelection(int deltaCol, int deltaRow)
    {
        if (!isPlaying || isPaused) return;

        if (cellUIs != null && cellUIs[selectedRow, selectedCol] != null)
        {
            cellUIs[selectedRow, selectedCol].SetSelected(false);
        }

        selectedCol += deltaCol;
        selectedRow += deltaRow;

        if (selectedCol < 0) selectedCol = columns - 1;
        if (selectedCol >= columns) selectedCol = 0;
        if (selectedRow < 0) selectedRow = rows - 1;
        if (selectedRow >= rows) selectedRow = 0;

        UpdateSelection();
        PlaySound2D(moveSound, 0.3f);
    }

    private void UpdateSelection()
    {
        if (cellUIs == null) return;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cellUIs[r, c] != null)
                    cellUIs[r, c].SetSelected(false);
            }
        }

        if (cellUIs[selectedRow, selectedCol] != null)
        {
            cellUIs[selectedRow, selectedCol].SetSelected(true);
        }
    }

    public void PlacePiece()
    {
        if (!isPlaying || isPaused) return;

        string currentPlayer = caroLogic.CurrentPlayer;
        if (currentPlayer != "x") return;

        MakeMove(selectedRow, selectedCol, currentPlayer);
    }

    public void TogglePause()
    {
        if (isPaused)
            OnResume();
        else
            OnPause();
    }

    public void ExitGame()
    {
        OnExit();
    }

    #endregion

    #region Game Logic

    public bool MakeMove(int row, int col, string player)
    {
        if (!isPlaying || isPaused) return false;

        if (!caroLogic.MakeMove(row, col))
            return false;

        if (cellUIs[row, col] != null)
            cellUIs[row, col].UpdateVisual(player);

        PlaySound2D(moveSound, 0.5f);

        if (caroLogic.CheckWin(row, col))
        {
            StartCoroutine(OnGameEnd(player, true));
            return true;
        }

        if (caroLogic.IsDraw())
        {
            StartCoroutine(OnGameEnd("", false));
            return true;
        }

        caroLogic.SwitchPlayer();

        if (caroLogic.CurrentPlayer == "o" && botPlayer != null)
        {
            StartCoroutine(TriggerBotMove());
        }

        return true;
    }

    private IEnumerator TriggerBotMove()
    {
        //Debug.Log("[CaroMinigame] Bot is making a move...");
        yield return new WaitForSeconds(0.5f);
        if (botPlayer && isPlaying && !isPaused)
        {
            botPlayer.MakeMove();
        }
        //Debug.Log("[CaroMinigame] Bot has made its move.");
    }

    private IEnumerator OnGameEnd(string winner, bool isWin)
    {
        isPlaying = false;
        DisableAllCells();

        if (isWin)
        {
            PlaySound2D(winSound, 1f);

            if (winPanel)
            {
                winPanel.SetActive(true);
            }

            if (winner == "x")
                CompleteSuccess();
        }
        else
        {
            PlaySound2D(drawSound, 0.8f);

            if (winPanel)
            {
                winPanel.SetActive(true);
            }
        }

        yield return new WaitForSeconds(2f);
        OnExit();
    }

    private void DisableAllCells()
    {
        if (cellUIs == null) return;

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                if (cellUIs[i, j] != null)
                {
                    var button = cellUIs[i, j].GetComponent<Button>();
                    if (button) button.interactable = false;
                }
    }

    #endregion

    #region Public API for Bot

    public string GetCurrentPlayer() => caroLogic.CurrentPlayer;
    public string GetMatrixValue(int row, int col) => caroLogic.GetCell(row, col);
    public bool IsCellEmpty(int row, int col) => caroLogic.IsCellEmpty(row, col);
    public bool IsOutOfBounds(int row, int col) => caroLogic.IsOutOfBounds(row, col);
    public List<(int row, int col)> GetAllEmptyCells() => caroLogic.GetAllEmptyCells();
    public bool TryMoveAndCheckWin(int row, int col, string symbol) => caroLogic.TryMoveAndCheckWin(row, col, symbol);
    public bool CheckWin(int row, int col) => caroLogic.CheckWin(row, col);

    #endregion

    #region Utilities

    private void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip && audioService != null)
        {
            audioService.PlaySound2D(clip, volume);
        }
    }

    #endregion
}