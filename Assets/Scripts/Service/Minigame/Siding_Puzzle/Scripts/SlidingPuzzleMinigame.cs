using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SlidingPuzzleMinigame : MinigameBase
{
    [Header("Puzzle References")]
    [SerializeField] private GameObject[] piecePrefabs = new GameObject[8];
    [SerializeField] private Transform boardContainer;

    [Header("Grid Settings")]
    [SerializeField] private Vector2 cellSize = new Vector2(2f, 2f);
    [SerializeField] private float spacing = 0.2f;

    [Header("Game Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip selectSound;

    [Header("UI")]
    [SerializeField] private SlidingPuzzleUI puzzleUI;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = Color.yellow;

    private SlidingPuzzleLogic puzzleLogic;
    private GameObject[,] pieceObjects;
    private bool isMoving = false;
    private bool isPlaying = false;

    private int moveCount = 0;
    private float gameStartTime;

    // NEW: Selection state
    private int selectedRow = 0;
    private int selectedCol = 0;
    private IHighlighter highlighter;

    protected override void Start()
    {
        base.Start();
        ValidatePrefabs();
        highlighter = new EmissionHighlighter();
    }

    private void ValidatePrefabs()
    {
        if (piecePrefabs.Length != 8)
        {
            Debug.LogError($"[SlidingPuzzle] Cần đúng 8 prefabs! Hiện tại: {piecePrefabs.Length}");
            return;
        }

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (piecePrefabs[i] == null)
            {
                Debug.LogError($"[SlidingPuzzle] Prefab {i + 1} chưa được gán!");
            }
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();

        puzzleLogic = new SlidingPuzzleLogic();
        //puzzleLogic.Shuffle(20);
        GeneratePiecesUI();

        moveCount = 0;
        gameStartTime = Time.time;
        isPlaying = true;

        selectedRow = 0;
        selectedCol = 0;
        HighlightSelectedPiece();

        if (puzzleUI != null)
            puzzleUI.Reset();
    }

    public override void OnExit()
    {
        ClearHighlight();
        base.OnExit();
        isPlaying = false;
        ClearPieces();
    }

    public override void OnPause()
    {
        ClearHighlight();
        base.OnPause();
    }

    public override void OnResume()
    {
        base.OnResume();
        HighlightSelectedPiece();
    }

    #region Input Overrides
    protected override void OnUpPressed()
    {
        if (!isPlaying || isMoving) return;
        MoveSelection(-1, 0);
    }

    protected override void OnDownPressed()
    {
        if (!isPlaying || isMoving) return;
        MoveSelection(1, 0);
    }

    protected override void OnLeftPressed()
    {
        if (!isPlaying || isMoving) return;
        MoveSelection(0, 1);
    }

    protected override void OnRightPressed()
    {
        if (!isPlaying || isMoving) return;
        MoveSelection(0, -1);
    }
    protected override void OnIncreasePressed()
    {
        if (!isPlaying || isMoving) return;
        TryMoveSelectedPiece();
    }

    protected override void OnDecreasePressed()
    {
        if (!isPlaying || isMoving) return;
        TryMoveSelectedPiece();
    }

    protected override void OnResetPressed()
    {
        if (!isPlaying || isMoving) return;
        ResetPuzzle();
    }

    protected override void OnCancelPressed()
    {
        if (!isPlaying) return;
        OnPause();
    }

    #endregion

    #region Selection Logic // NEW: Selection system

    private void MoveSelection(int rowOffset, int colOffset)
    {
        int newRow = selectedRow + rowOffset;
        int newCol = selectedCol + colOffset;

        if (newRow < 0 || newRow >= puzzleLogic.Rows || newCol < 0 || newCol >= puzzleLogic.Cols)
            return;

        if (newRow == puzzleLogic.EmptyRow && newCol == puzzleLogic.EmptyCol)
            return;

        ClearHighlight();
        selectedRow = newRow;
        selectedCol = newCol;
        HighlightSelectedPiece();

        PlaySound2D(selectSound, 0.3f);
    }

    private void HighlightSelectedPiece()
    {
        GameObject selectedPiece = pieceObjects[selectedRow, selectedCol];
        if (selectedPiece != null && highlighter != null)
        {
            highlighter.Highlight(selectedPiece, highlightColor);
        }
    }

    private void ClearHighlight()
    {
        GameObject selectedPiece = pieceObjects[selectedRow, selectedCol];
        if (selectedPiece != null && highlighter != null)
        {
            highlighter.RemoveHighlight(selectedPiece);
        }
    }

    private void TryMoveSelectedPiece()
    {
        int emptyRow = puzzleLogic.EmptyRow;
        int emptyCol = puzzleLogic.EmptyCol;

        bool isAdjacent = (Mathf.Abs(selectedRow - emptyRow) == 1 && selectedCol == emptyCol) ||
                          (Mathf.Abs(selectedCol - emptyCol) == 1 && selectedRow == emptyRow);

        if (!isAdjacent)
        {
            Debug.Log("[SlidingPuzzle] Piece không thể di chuyển (không kề empty cell)");
            return;
        }

        GameObject pieceToMove = pieceObjects[selectedRow, selectedCol];
        if (pieceToMove == null)
        {
            Debug.LogWarning("[SlidingPuzzle] No piece to move!");
            return;
        }

        ClearHighlight();
        StartCoroutine(MovePieceAnimation(pieceToMove, selectedRow, selectedCol));
    }

    #endregion

    #region Pieces UI

    private void GeneratePiecesUI()
    {
        ClearPieces();

        pieceObjects = new GameObject[puzzleLogic.Rows, puzzleLogic.Cols];
        int pieceIndex = 0;

        for (int row = 0; row < puzzleLogic.Rows; row++)
        {
            for (int col = 0; col < puzzleLogic.Cols; col++)
            {
                if (row == 2 && col == 2)
                {
                    pieceObjects[row, col] = null;
                    continue;
                }

                Vector3 position = CalculateWorldPosition(row, col);
                GameObject piece = Instantiate(
                    piecePrefabs[pieceIndex],
                    position,
                    Quaternion.Euler(90f,180f,0f),
                    boardContainer);
                piece.name = $"Piece_{pieceIndex + 1}_[{row},{col}]";

                pieceObjects[row, col] = piece;
                pieceIndex++;
            }
        }

        Debug.Log("[SlidingPuzzle] Pieces generated");
    }

    private void ClearPieces()
    {
        if (boardContainer == null) return;

        foreach (Transform child in boardContainer)
        {
            Destroy(child.gameObject);
        }

        pieceObjects = null;
    }

    private Vector3 CalculateWorldPosition(int row, int col)
    {
        float x = col * spacing;
        float y = -row * spacing;

        float offsetX = -(puzzleLogic.Cols - 1) * spacing / 2f;
        float offsetY = (puzzleLogic.Rows - 1) * spacing / 2f;

        Vector3 localPosition = new (x + offsetX, y + offsetY,0f);
        return boardContainer.TransformPoint(localPosition);
        return boardContainer.position + new Vector3(x + offsetX, y + offsetY, 0f);
    }

    #endregion

    #region Game Logic

    private IEnumerator MovePieceAnimation(GameObject piece, int fromRow, int fromCol)
    {
        int oldEmptyRow = puzzleLogic.EmptyRow;
        int oldEmptyCol = puzzleLogic.EmptyCol;
        isMoving = true;

        Vector3 targetPos = CalculateWorldPosition(puzzleLogic.EmptyRow, puzzleLogic.EmptyCol);
        float duration = 1f / moveSpeed;

        yield return piece.transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad).WaitForCompletion();

        puzzleLogic.MakeMove(fromRow, fromCol);

        pieceObjects[oldEmptyRow, oldEmptyCol] = piece;
        pieceObjects[fromRow, fromCol] = null;

        int pieceIndex = GetPieceIndex(piece);
        piece.name = $"Piece_{pieceIndex + 1}_[{puzzleLogic.EmptyRow},{puzzleLogic.EmptyCol}]";

        selectedRow = oldEmptyRow;
        selectedCol = oldEmptyCol;

        moveCount++;
        if (puzzleUI != null)
            puzzleUI.UpdateMoves(moveCount);

        PlaySound2D(moveSound, 0.5f);

        isMoving = false;

        HighlightSelectedPiece();

        if (puzzleLogic.CheckWin())
        {
            StartCoroutine(OnGameWin());
        }
    }

    private int GetPieceIndex(GameObject piece)
    {
        string name = piece.name;
        int startIndex = name.IndexOf("Piece_") + 6;
        int endIndex = name.IndexOf("_", startIndex);
        return int.Parse(name.Substring(startIndex, endIndex - startIndex)) - 1;
    }

    private IEnumerator OnGameWin()
    {
        isPlaying = false;
        ClearHighlight();

        PlaySound2D(winSound, 1f);

        float gameTime = Time.time - gameStartTime;
        if (puzzleUI != null)
            puzzleUI.ShowWin(moveCount, gameTime);

        Debug.Log($"[SlidingPuzzle] WIN! Moves: {moveCount}, Time: {gameTime:F1}s");

        yield return new WaitForSeconds(2f);

        CompleteSuccess();
        OnExit();
    }

    private void ResetPuzzle()
    {
        if (isMoving) return;

        Debug.Log("[SlidingPuzzle] Resetting puzzle...");

        ClearHighlight();

        moveCount = 0;
        gameStartTime = Time.time;
        if (puzzleUI != null)
            puzzleUI.Reset();

        puzzleLogic.Reset();
        UpdateAllPiecePositions();

        selectedRow = 0;
        selectedCol = 0;
        HighlightSelectedPiece();

        Debug.Log("[SlidingPuzzle] Reset complete!");
    }

    private void UpdateAllPiecePositions()
    {
        int pieceIndex = 0;
        for (int row = 0; row < puzzleLogic.Rows; row++)
        {
            for (int col = 0; col < puzzleLogic.Cols; col++)
            {
                if (row == 2 && col == 2)
                {
                    pieceObjects[row, col] = null;
                    continue;
                }

                GameObject piece = FindPieceByIndex(pieceIndex);
                if (piece != null)
                {
                    piece.transform.position = CalculateWorldPosition(row, col);
                    pieceObjects[row, col] = piece;
                    piece.name = $"Piece_{pieceIndex + 1}_[{row},{col}]";
                }

                pieceIndex++;
            }
        }
    }

    private GameObject FindPieceByIndex(int index)
    {
        string pieceName = $"Piece_{index + 1}_";
        foreach (Transform child in boardContainer)
        {
            if (child.name.StartsWith(pieceName))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    #endregion

    #region Public API for UI

    public int GetMoveCount() => moveCount;
    public float GetGameTime() => Time.time - gameStartTime;
    public bool IsGameWon() => puzzleLogic != null && puzzleLogic.CheckWin();

    #endregion

    private void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip && audioService != null)
        {
            audioService.PlaySound2D(clip, volume);
        }
    }

    private void Update()
    {
        if (isPlaying && puzzleUI != null)
        {
            puzzleUI.UpdateTime(GetGameTime());
        }
    }
}