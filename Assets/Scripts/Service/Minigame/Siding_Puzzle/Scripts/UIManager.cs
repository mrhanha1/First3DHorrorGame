using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public GameManager puzzleManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timeText;
    
    [Header("Canvas Settings")]
    [Tooltip("Canvas sẽ được tạo tự động nếu để trống")]
    public Canvas worldCanvas;
    
    
    [Tooltip("Kích thước canvas")]
    public Vector2 canvasSize = new Vector2(800f, 200f);
    
    [Header("Text Settings")]
    [Tooltip("Kích thước chữ")]
    public float fontSize = 48f;
    
    [Tooltip("Màu chữ")]
    public Color textColor = Color.white;
    
    [Tooltip("Màu chữ khi thắng")]
    public Color winColor = Color.yellow;
    
    private bool gameWon = false;
    
    void Start()
    {
        if (puzzleManager == null)
        {
            puzzleManager = GetComponent<GameManager>();
        }
        
        SetupTextStyle();
    }
    
   
    
    
    void SetupTextStyle()
    {
        if (movesText != null)
        {
            movesText.fontSize = fontSize;
            movesText.color = textColor;
            movesText.alignment = TextAlignmentOptions.Center;
            movesText.fontStyle = FontStyles.Bold;
            movesText.text = "Moves: 0";
            
            // Outline cho dễ đọc
            movesText.outlineWidth = 0.2f;
            movesText.outlineColor = Color.black;
        }
        
        if (timeText != null)
        {
            timeText.fontSize = fontSize;
            timeText.color = textColor;
            timeText.alignment = TextAlignmentOptions.Center;
            timeText.fontStyle = FontStyles.Bold;
            timeText.text = "Time: 0.0s";
            
            // Outline cho dễ đọc
            timeText.outlineWidth = 0.2f;
            timeText.outlineColor = Color.black;
        }
    }
    
    void Update()
    {
        if (puzzleManager == null) return;
        
        UpdateUI();
        
        // Quay canvas về phía camera
        if (worldCanvas != null && Camera.main != null)
        {
            worldCanvas.transform.LookAt(Camera.main.transform);
            worldCanvas.transform.Rotate(0f, 180f, 0f); // Đảo ngược để text đọc được
        }
    }
    
    void UpdateUI()
    {
        // Kiểm tra xem game đã thắng chưa
        bool currentWinState = CheckIfWon();
        
        if (currentWinState && !gameWon)
        {
            gameWon = true;
            ShowWinUI();
            return;
        }
        
        // Update Moves
        if (movesText != null)
        {
            int moves = GetMoveCount();
            movesText.text = $"Moves: {moves}";
        }
        
        // Update Time
        if (timeText != null && !gameWon)
        {
            float time = GetGameTime();
            timeText.text = $"Time: {time:F1}s";
        }
    }
    
    void ShowWinUI()
    {
        if (movesText != null)
        {
            movesText.color = winColor;
        }
        
        if (timeText != null)
        {
            timeText.color = winColor;
            float finalTime = GetGameTime();
            timeText.text = $"★ COMPLETED! ★\nTime: {finalTime:F1}s";
        }
        
    }
    
    
    // Helper methods để lấy thông tin từ SimplePuzzleManager
    int GetMoveCount()
    {
        // Dùng reflection để lấy private field
        var field = puzzleManager.GetType().GetField("moveCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (int)field.GetValue(puzzleManager);
        }
        return 0;
    }
    
    float GetGameTime()
    {
        var startTimeField = puzzleManager.GetType().GetField("gameStartTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (startTimeField != null)
        {
            float startTime = (float)startTimeField.GetValue(puzzleManager);
            return Time.time - startTime;
        }
        return 0f;
    }
    
    bool CheckIfWon()
    {
        var field = puzzleManager.GetType().GetField("gameWon", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (bool)field.GetValue(puzzleManager);
        }
        return false;
    }
    
    public void ResetUI()
    {
        gameWon = false;
        
        if (movesText != null)
        {
            movesText.color = textColor;
            movesText.fontSize = fontSize;
            movesText.text = "Moves: 0";
        }
        
        if (timeText != null)
        {
            timeText.color = textColor;
            timeText.fontSize = fontSize;
            timeText.text = "Time: 0.0s";
        }
    }
}