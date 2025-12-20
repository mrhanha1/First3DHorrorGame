using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI slotNumberText;
    [SerializeField] private TextMeshProUGUI saveNameText;
    [SerializeField] private TextMeshProUGUI saveDateText;
    [SerializeField] private GameObject emptySlotIndicator;
    [SerializeField] private GameObject filledSlotContent;
    [SerializeField] private Button slotButton;
    [SerializeField] private Button deleteButton;

    [Header("Visual Settings")]
    [SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    [SerializeField] private Color filledSlotColor = new Color(0.2f, 0.5f, 0.8f, 0.9f);
    [SerializeField] private Color disabledSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    private SaveSlotData slotData;
    private SaveMenuUI menuUI;
    private Image backgroundImage;

    private void Awake()
    {
        menuUI = GetComponentInParent<SaveMenuUI>();
        backgroundImage = GetComponent<Image>();

        if (slotButton != null)
            slotButton.onClick.AddListener(OnSlotClicked);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    public void UpdateSlotData(SaveSlotData data, bool isSaveMode)
    {
        slotData = data;

        if (slotNumberText != null)
        {
            slotNumberText.text = $"{data.slotIndex + 1}";
        }

        bool isInteractable = isSaveMode || !data.isEmpty;
        UpdateSlotVisuals(data.isEmpty, isInteractable, isSaveMode);

        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(!data.isEmpty);
        }
    }

    private void UpdateSlotVisuals(bool isEmpty, bool isInteractable, bool isSaveMode)
    {
        if (emptySlotIndicator != null)
        {
            emptySlotIndicator.SetActive(isEmpty);
            if (isEmpty)
            {
                TextMeshProUGUI emptyText = emptySlotIndicator.GetComponent<TextMeshProUGUI>();
                if (emptyText != null)
                {
                    emptyText.text = isSaveMode ? "Bản lưu trống" : "Không có dữ liệu";
                }
            }
        }

        if (filledSlotContent != null)
        {
            filledSlotContent.SetActive(!isEmpty);
        }

        if (!isEmpty)
        {
            if (saveNameText != null)
                saveNameText.text = slotData.saveName;

            if (saveDateText != null)
                saveDateText.text = slotData.saveDate;
        }

        if (slotButton != null)
            slotButton.interactable = isInteractable;

        if (backgroundImage != null)
        {
            if (!isInteractable)
            {
                backgroundImage.color = disabledSlotColor;
            }
            else
            {
                backgroundImage.color = isEmpty ? emptySlotColor : filledSlotColor;
            }
        }
    }

    private void OnSlotClicked()
    {
        if (menuUI != null)
        {
            menuUI.OnSlotClicked(slotData.slotIndex);
        }
    }

    private void OnDeleteClicked()
    {
        if (menuUI != null)
        {
            menuUI.OnDeleteSlot(slotData.slotIndex);
        }
    }
}