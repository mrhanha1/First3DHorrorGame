using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Column { get; set; }
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    private Image image;
    private Button button;
    private ChessBoard board;

    private void Awake()
    {
        if (!TryGetComponent<Image>(out image)) Debug.LogError("Image component missing!", this);
        if (!TryGetComponent<Button>(out button)) Debug.LogError("Button component missing!", this);
        else button.onClick.AddListener(OnClick);
    }

    public void Initialize(ChessBoard chessBoard)
    {
        if (chessBoard == null) Debug.LogError("ChessBoard is null!", this);
        board = chessBoard;
    }

    public void ChangeImage(string player)
    {
        image.sprite = player switch
        {
            "x" => xSprite,
            "o" => oSprite,
            _ => null
        };
        button.interactable = string.IsNullOrEmpty(player);
        if (image.sprite == null && !string.IsNullOrEmpty(player))
            Debug.LogWarning($"Sprite for player '{player}' not assigned at [{Row}, {Column}]!", this);
    }

    private void OnClick()
    {
        if (board == null) { Debug.LogError("ChessBoard not initialized!", this); return; }

        string currentPlayer = board.CurrentPlayer;
        if (!board.MakeMove(Row, Column, currentPlayer))
        {
            Debug.LogWarning($"Invalid move at [{Row}, {Column}]!", this);
            return;
        }

        ChangeImage(currentPlayer);
    }

    private void OnDestroy()
    {
        if (button) button.onClick.RemoveListener(OnClick);
    }
}
