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

    /// <summary>
    /// C?p nh?t hi?n th? slot v?i d? li?u m?i
    /// </summary>
    public void UpdateSlotData(SaveSlotData data, bool isSaveMode)
    {
        slotData = data;

        // C?p nh?t s? slot
        if (slotNumberText != null)
        {
            slotNumberText.text = $"SLOT {data.slotIndex + 1}";
        }

        if (data.isEmpty)
        {
            ShowEmptySlot(isSaveMode);
        }
        else
        {
            ShowFilledSlot();
        }

        // Hi?n th?/?n nút delete
        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(!data.isEmpty);
        }
    }

    private void ShowEmptySlot(bool isSaveMode)
    {
        // Hi?n th? text "Empty Slot"
        if (emptySlotIndicator != null)
        {
            emptySlotIndicator.SetActive(true);
            TextMeshProUGUI emptyText = emptySlotIndicator.GetComponent<TextMeshProUGUI>();
            if (emptyText != null)
            {
                emptyText.text = isSaveMode ? "< Empty Slot >" : "< No Save Data >";
            }
        }

        // ?n n?i dung slot ??y
        if (filledSlotContent != null)
            filledSlotContent.SetActive(false);

        // Trong Load mode, disable slot tr?ng
        bool isInteractable = isSaveMode;
        if (slotButton != null)
            slotButton.interactable = isInteractable;

        // ??i màu background
        if (backgroundImage != null)
        {
            backgroundImage.color = isInteractable ? emptySlotColor : disabledSlotColor;
        }
    }

    private void ShowFilledSlot()
    {
        // ?n text "Empty Slot"
        if (emptySlotIndicator != null)
            emptySlotIndicator.SetActive(false);

        // Hi?n th? n?i dung slot
        if (filledSlotContent != null)
            filledSlotContent.SetActive(true);

        // Hi?n th? tên save
        if (saveNameText != null)
            saveNameText.text = slotData.saveName;

        // Hi?n th? ngày gi?
        if (saveDateText != null)
            saveDateText.text = slotData.saveDate;

        // Enable button
        if (slotButton != null)
            slotButton.interactable = true;

        // ??i màu background
        if (backgroundImage != null)
        {
            backgroundImage.color = filledSlotColor;
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