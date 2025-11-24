using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component cho m?t ô Sudoku
/// </summary>
public class SudokuCellUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text numberText;
    [SerializeField] private Image background;
    [SerializeField] private Image border;

    [Header("Colors")]
    [SerializeField] private Color fixedColor = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color emptyColor = Color.white;
    [SerializeField] private Color errorColor = new Color(1f, 0.7f, 0.7f);
    [SerializeField] private Color correctColor = new Color(0.9f, 1f, 0.9f);

    [Header("Border")]
    [SerializeField] private Color normalBorderColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color selectedBorderColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private float normalBorderWidth = 2f;
    [SerializeField] private float selectedBorderWidth = 4f;

    private SudokuCell cellData;
    private bool isSelected = false;

    public SudokuCell CellData => cellData;
    public bool IsSelected => isSelected;

    private void Awake()
    {
        if (numberText == null) numberText = GetComponentInChildren<Text>();
        if (background == null) background = GetComponent<Image>();

        // Find border (child named "Border")
        if (border == null)
        {
            Transform borderTransform = transform.Find("Border");
            if (borderTransform != null)
                border = borderTransform.GetComponent<Image>();
        }
    }

    public void Initialize(SudokuCell cell)
    {
        this.cellData = cell;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (cellData == null) return;

        // Update number text
        if (numberText)
        {
            numberText.text = cellData.value > 0 ? cellData.value.ToString() : "";

            if (cellData.isFixed)
            {
                numberText.color = Color.black;
                numberText.fontStyle = FontStyle.Bold;
            }
            else
            {
                numberText.color = new Color(0.2f, 0.2f, 0.8f);
                numberText.fontStyle = FontStyle.Normal;
            }
        }

        // Update background color
        if (background)
        {
            if (cellData.isFixed)
            {
                background.color = fixedColor;
            }
            else if (!cellData.IsEmpty && !cellData.IsValid)
            {
                background.color = errorColor;
            }
            else if (!cellData.IsEmpty && cellData.IsValid)
            {
                background.color = correctColor;
            }
            else
            {
                background.color = emptyColor;
            }
        }

        // Update border
        UpdateBorder();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBorder();
    }

    private void UpdateBorder()
    {
        if (border == null) return;

        if (isSelected)
        {
            border.color = selectedBorderColor;

            // Make border thicker
            RectTransform rectTransform = border.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.offsetMin = Vector2.one * -selectedBorderWidth;
                rectTransform.offsetMax = Vector2.one * selectedBorderWidth;
            }
        }
        else
        {
            border.color = normalBorderColor;

            // Normal border
            RectTransform rectTransform = border.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.offsetMin = Vector2.one * -normalBorderWidth;
                rectTransform.offsetMax = Vector2.one * normalBorderWidth;
            }
        }
    }

    public void SetValue(int value)
    {
        cellData?.SetValue(value);
        UpdateVisuals();
    }

    public void IncreaseValue()
    {
        cellData?.IncreaseValue();
        UpdateVisuals();
    }

    public void DecreaseValue()
    {
        cellData?.DecreaseValue();
        UpdateVisuals();
    }
}