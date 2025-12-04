using UnityEngine;

/// <summary>
/// Sliding Puzzle kế thừa từ MinigameBase
/// Kết nối với GameManager để xử lý logic puzzle
/// </summary>
public class SidingPuzzleMinigame : MinigameBase
{
    [Header("Puzzle References")]
    [SerializeField] private GameManager gameManager;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Tìm GameManager nếu chưa gán
        if (gameManager == null)
        {
            gameManager = GetComponent<GameManager>();
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        
        if (gameManager == null)
        {
            Debug.LogError("[SlidingPuzzleMinigame] GameManager not found!");
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        // Kích hoạt virtual camera cho minigame
        if (virtualCamera != null)
        {
            virtualCamera.Priority = 20;
        }
        
        // Shuffle puzzle khi vào game
        if (gameManager != null)
        {
            gameManager.ShufflePuzzle();
        }
        
        Debug.Log("[SlidingPuzzleMinigame] Entered - Camera activated");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        // Kiểm tra điều kiện thắng
        if (gameManager != null && gameManager.IsGameWon())
        {
            CompleteSuccess();
        }
    }

    public override void OnExit()
    {
        // Reset camera priority về mặc định
        if (virtualCamera != null)
        {
            virtualCamera.Priority = 1;
        }
        
        base.OnExit();
        
        Debug.Log("[SlidingPuzzleMinigame] Exited - Camera deactivated");
    }

    #region Input Overrides - Ánh xạ input sang GameManager

    protected override void OnUpPressed()
    {
        if (!isActive || gameManager == null) return;
        // Logic di chuyển lên (piece bên dưới lên)
        gameManager.TryMovePiece(1, 0);
    }

    protected override void OnDownPressed()
    {
        if (!isActive || gameManager == null) return;
        // Logic di chuyển xuống (piece bên trên xuống)
        gameManager.TryMovePiece(-1, 0);
    }

    protected override void OnLeftPressed()
    {
        if (!isActive || gameManager == null) return;
        // Logic di chuyển trái (piece bên phải sang trái)
        gameManager.TryMovePiece(0, 1);
    }

    protected override void OnRightPressed()
    {
        if (!isActive || gameManager == null) return;
        // Logic di chuyển phải (piece bên trái sang phải)
        gameManager.TryMovePiece(0, -1);
    }

    protected override void OnResetPressed()
    {
        if (!isActive || gameManager == null) return;
        gameManager.ResetPuzzle();
    }

    protected override void OnSubmitPressed()
    {
        if (!isActive || gameManager == null) return;
        // Có thể dùng để shuffle
        gameManager.ShufflePuzzle();
    }

    #endregion
}