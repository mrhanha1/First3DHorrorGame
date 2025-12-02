using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int Row { get; set; }
    public int Column { get; set; }

    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private Image image;
    private Button button;
    private CaroMinigame caroMinigame;
    private bool isSelected = false;

    private void Awake()
    {
        if (!TryGetComponent<Image>(out image))
            Debug.LogError("Image component missing!", this);

        if (!TryGetComponent<Button>(out button))
            Debug.LogError("Button component missing!", this);
        else
            button.onClick.AddListener(OnClick);
    }

    public void Initialize(CaroMinigame minigame)
    {
        if (minigame == null)
            Debug.LogError("CaroMinigame is null!", this);

        caroMinigame = minigame;
    }

    public void UpdateVisual(string player)
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

        UpdateSelectionVisual();
    }

    public void ChangeImage(string player)
    {
        UpdateVisual(player);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateSelectionVisual();
    }

    private void UpdateSelectionVisual()
    {
        if (image == null) return;

        // Only show selection highlight if cell is empty
        if (string.IsNullOrEmpty(caroMinigame?.GetMatrixValue(Row, Column)))
        {
            image.color = isSelected ? selectedColor : normalColor;
        }
        else
        {
            image.color = normalColor;
        }
    }

    private void OnClick()
    {
        if (caroMinigame == null)
        {
            Debug.LogError("CaroMinigame not initialized!", this);
            return;
        }

        string currentPlayer = caroMinigame.GetCurrentPlayer();

        if (!caroMinigame.MakeMove(Row, Column, currentPlayer))
        {
            Debug.LogWarning($"Invalid move at [{Row}, {Column}]!", this);
            return;
        }

        UpdateVisual(currentPlayer);
    }

    private void OnDestroy()
    {
        if (button)
            button.onClick.RemoveListener(OnClick);
    }
}