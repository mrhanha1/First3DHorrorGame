using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject[] piecePrefabs = new GameObject[8];
    
    public Vector2 cellSize = new Vector2(2f, 2f);
    
    public float spacing = 0.2f;
    
    public KeyCode moveUp = KeyCode.W;
    
    public KeyCode moveDown = KeyCode.S;
    
    public KeyCode moveLeft = KeyCode.A;
    
    public KeyCode moveRight = KeyCode.D;
    
    public KeyCode shuffle = KeyCode.Space;
    
    public KeyCode reset = KeyCode.R;
    
    public int shuffleMoves = 30;
    
    public float moveSpeed = 10f;
    
    // Grid 3x3: [row, col]
    private GameObject[,] grid = new GameObject[3, 3];
    private int[,] correctGrid = new int[3, 3]; // Lưu vị trí đúng
    private int emptyRow = 2; // Ô trống ở vị trí [2,2]
    private int emptyCol = 2;
    
    private bool isMoving = false;
    private int moveCount = 0;
    private bool gameWon = false;
    private float gameStartTime;
    
    void Start()
    {
        InitializePuzzle();
        Invoke("ShufflePuzzle", 0.5f);
        gameStartTime = Time.time;
        
        PrintControls();
    }
    
    void InitializePuzzle()
    {
        if (piecePrefabs.Length != 8)
        {
            Debug.LogError("Cần đúng 8 prefabs! Hiện tại có: " + piecePrefabs.Length);
            return;
        }
        
        // Kiểm tra prefabs có null không
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (piecePrefabs[i] == null)
            {
                Debug.LogError($"Prefab {i + 1} chưa được gán!");
                return;
            }
        }
        
        // Tạo 8 ô đầu (ô thứ 9 là ô trống)
        int pieceIndex = 0;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // Ô cuối là ô trống
                if (row == 2 && col == 2)
                {
                    grid[row, col] = null;
                    correctGrid[row, col] = -1;
                    continue;
                }
                
                // Tạo piece từ prefab
                Vector3 position = CalculateWorldPosition(row, col);
                GameObject piece = Instantiate(piecePrefabs[pieceIndex], position, Quaternion.identity, transform);
                piece.name = $"Piece_{pieceIndex + 1}_[{row},{col}]";
                
                grid[row, col] = piece;
                correctGrid[row, col] = pieceIndex;
                
                pieceIndex++;
            }
        }
        
        Debug.Log("✓ Đã tạo puzzle 3x3 với 8 prefabs + 1 ô trống");
    }
    
    Vector3 CalculateWorldPosition(int row, int col)
    {
        float totalWidth = 2 * (cellSize.x + spacing);
        float totalHeight = 2 * (cellSize.y + spacing);
        
        float x = col * (cellSize.x + spacing) - totalWidth / 2f;
        float y = -row * (cellSize.y + spacing) + totalHeight / 2f;
        
        return transform.position + new Vector3(x, y, 0f);
    }
    
    void Update()
    {
        if (gameWon || isMoving) return;
        
        // Điều khiển bằng phím
        if (Input.GetKeyDown(moveUp))
        {
            TryMovePiece(1, 0); // Di chuyển piece BÊN DƯỚI lên 
        }
        else if (Input.GetKeyDown(moveDown))
        {
            TryMovePiece(-1, 0); // Di chuyển piece BÊN TRÊN xuống 
        }
        else if (Input.GetKeyDown(moveLeft))
        {
            TryMovePiece(0, 1); // Di chuyển piece BÊN PHẢI sang trái 
        }
        else if (Input.GetKeyDown(moveRight))
        {
            TryMovePiece(0, -1); // Di chuyển piece BÊN TRÁI sang phải 
        }
        else if (Input.GetKeyDown(shuffle))
        {
            ShufflePuzzle();
        }
        else if (Input.GetKeyDown(reset))
        {
            ResetPuzzle();
        }
    }
    
    void TryMovePiece(int rowOffset, int colOffset)
    {
        // Tìm piece kế bên ô trống
        int targetRow = emptyRow + rowOffset;
        int targetCol = emptyCol + colOffset;
        
        // Kiểm tra bounds
        if (targetRow < 0 || targetRow >= 3 || targetCol < 0 || targetCol >= 3)
        {
            Debug.Log("Không thể di chuyển ra ngoài!");
            return;
        }
        
        GameObject pieceToMove = grid[targetRow, targetCol];
        if (pieceToMove == null)
        {
            Debug.LogWarning("Không có piece để di chuyển!");
            return;
        }
        
        // Di chuyển piece
        StartCoroutine(MovePieceAnimation(pieceToMove, targetRow, targetCol));
    }
    
    System.Collections.IEnumerator MovePieceAnimation(GameObject piece, int fromRow, int fromCol)
    {
        isMoving = true;
        
        Vector3 startPos = piece.transform.position;
        Vector3 targetPos = CalculateWorldPosition(emptyRow, emptyCol);
        
        float elapsed = 0f;
        float duration = 1f / moveSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            piece.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        piece.transform.position = targetPos;
        
        // Cập nhật grid
        grid[emptyRow, emptyCol] = piece;
        grid[fromRow, fromCol] = null;
        
        // Cập nhật tên piece
        piece.name = piece.name.Split('[')[0] + $"[{emptyRow},{emptyCol}]";
        
        // Ô trống di chuyển đến vị trí cũ của piece
        emptyRow = fromRow;
        emptyCol = fromCol;
        
        moveCount++;
        isMoving = false;
        
        // Kiểm tra thắng
        CheckWinCondition();
    }
    
    void CheckWinCondition()
    {
        int pieceIndex = 0;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // Ô trống
                if (row == 2 && col == 2)
                {
                    if (grid[row, col] != null)
                        return; // Chưa thắng
                    continue;
                }
                
                // Kiểm tra piece
                GameObject piece = grid[row, col];
                if (piece == null || piece != piecePrefabs[pieceIndex].gameObject && 
                    !piece.name.StartsWith($"Piece_{pieceIndex + 1}"))
                {
                    return; // Chưa thắng
                }
                
                pieceIndex++;
            }
        }
        
        // Thắng!
        WinGame();
    }
    
    void WinGame()
    {
        gameWon = true;
        float gameTime = Time.time - gameStartTime;
        
        Debug.Log($"<color=green>★★★ CHIẾN THẮNG! ★★★</color>\n" +
                  $"Số bước: {moveCount}\n" +
                  $"Thời gian: {gameTime:F1}s");
        
        // Hiệu ứng thắng
        StartCoroutine(WinAnimation());
    }
    
    System.Collections.IEnumerator WinAnimation()
    {
        for (int i = 0; i < 3; i++)
        {
            // Scale up
            foreach (var piece in grid)
            {
                if (piece != null)
                {
                    piece.transform.localScale = Vector3.one * 1.2f;
                }
            }
            yield return new WaitForSeconds(0.3f);
            
            // Scale down
            foreach (var piece in grid)
            {
                if (piece != null)
                {
                    piece.transform.localScale = Vector3.one;
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public void ShufflePuzzle()
    {
        if (isMoving) return;
        
        Debug.Log($"Đang shuffle {shuffleMoves} bước...");
        
        gameWon = false;
        moveCount = 0;
        gameStartTime = Time.time;
        
        for (int i = 0; i < shuffleMoves; i++)
        {
            List<Vector2Int> validMoves = GetValidMoves();
            if (validMoves.Count > 0)
            {
                Vector2Int move = validMoves[Random.Range(0, validMoves.Count)];
                MovePieceInstant(move.x, move.y);
            }
        }
        
        Debug.Log("✓ Shuffle hoàn tất! Bắt đầu chơi.");
    }
    
    List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        
        for (int i = 0; i < 4; i++)
        {
            int newRow = emptyRow + dx[i];
            int newCol = emptyCol + dy[i];
            
            if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
            {
                moves.Add(new Vector2Int(newRow, newCol));
            }
        }
        
        return moves;
    }
    
    void MovePieceInstant(int fromRow, int fromCol)
    {
        GameObject piece = grid[fromRow, fromCol];
        if (piece == null) return;
        
        Vector3 newPos = CalculateWorldPosition(emptyRow, emptyCol);
        piece.transform.position = newPos;
        
        grid[emptyRow, emptyCol] = piece;
        grid[fromRow, fromCol] = null;
        
        piece.name = piece.name.Split('[')[0] + $"[{emptyRow},{emptyCol}]";
        
        emptyRow = fromRow;
        emptyCol = fromCol;
    }
    
    public void ResetPuzzle()
    {
        Debug.Log("Đang reset puzzle...");
        
        gameWon = false;
        moveCount = 0;
        gameStartTime = Time.time;
        
        // Đưa tất cả pieces về đúng vị trí ban đầu
        int pieceIndex = 0;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (row == 2 && col == 2)
                {
                    grid[row, col] = null;
                    continue;
                }
                
                // Tìm piece đúng
                GameObject correctPiece = FindPieceByIndex(pieceIndex);
                if (correctPiece != null)
                {
                    correctPiece.transform.position = CalculateWorldPosition(row, col);
                    grid[row, col] = correctPiece;
                    correctPiece.name = $"Piece_{pieceIndex + 1}_[{row},{col}]";
                }
                
                pieceIndex++;
            }
        }
        
        emptyRow = 2;
        emptyCol = 2;
        
        Debug.Log("✓ Đã reset về vị trí ban đầu!");
    }
    
    GameObject FindPieceByIndex(int index)
    {
        string pieceName = $"Piece_{index + 1}_";
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith(pieceName))
            {
                return child.gameObject;
            }
        }
        return null;
    }
    
    void PrintControls()
    {
        Debug.Log($"<color=cyan>🎮 ĐIỀU KHIỂN:</color>\n" +
                  $"• [{moveUp}]: Di chuyển lên\n" +
                  $"• [{moveDown}]: Di chuyển xuống\n" +
                  $"• [{moveLeft}]: Di chuyển trái\n" +
                  $"• [{moveRight}]: Di chuyển phải\n" +
                  $"• [{shuffle}]: Shuffle puzzle\n" +
                  $"• [{reset}]: Reset về ban đầu");
    }
    
    void OnGUI()
    {
        int padding = 10;
        int lineHeight = 25;
        int y = padding;
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        
        GUIStyle shadowStyle = new GUIStyle(style);
        shadowStyle.normal.textColor = Color.black;
        
        // Moves
        string movesText = $"Moves: {moveCount}";
        GUI.Label(new Rect(padding + 1, y + 1, 200, lineHeight), movesText, shadowStyle);
        GUI.Label(new Rect(padding, y, 200, lineHeight), movesText, style);
        y += lineHeight;
        
        // Time
        if (!gameWon)
        {
            float time = Time.time - gameStartTime;
            string timeText = $"Time: {time:F1}s";
            GUI.Label(new Rect(padding + 1, y + 1, 200, lineHeight), timeText, shadowStyle);
            GUI.Label(new Rect(padding, y, 200, lineHeight), timeText, style);
        }
        else
        {
            string winText = "★ COMPLETED! ★";
            style.normal.textColor = Color.yellow;
            GUI.Label(new Rect(padding + 1, y + 1, 200, lineHeight), winText, shadowStyle);
            GUI.Label(new Rect(padding, y, 200, lineHeight), winText, style);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Vector3 pos = CalculateWorldPosition(row, col);
                Vector3 size = new Vector3(cellSize.x, cellSize.y, 0.5f);
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
}