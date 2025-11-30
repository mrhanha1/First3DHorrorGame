using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject saveMenuPanel;
    [SerializeField] private SaveSlotUI[] saveSlots; // 4 slots
    [SerializeField] private GameObject saveNameInputPanel;
    [SerializeField] private TMP_InputField saveNameInput;
    [SerializeField] private Button confirmSaveButton;
    [SerializeField] private Button cancelSaveButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Settings")]
    [SerializeField] private bool isSaveMode = true; // true = Save, false = Load

    private ISaveService saveService;
    private int selectedSlotIndex = -1;

    private void Awake()
    {
        // Get save service from ServiceLocator
        if (ServiceLocator.TryGet<ISaveService>(out ISaveService service))
        {
            saveService = service;
        }
        else
        {
            Debug.LogError("[SaveMenuUI] SaveService not found in ServiceLocator!");
        }

        // Setup button listeners
        if (confirmSaveButton != null)
            confirmSaveButton.onClick.AddListener(OnConfirmSave);

        if (cancelSaveButton != null)
            cancelSaveButton.onClick.AddListener(OnCancelSave);

        if (closeMenuButton != null)
            closeMenuButton.onClick.AddListener(HideSaveMenu);
    }

    private void OnEnable()
    {
        RefreshAllSlots();
        HideSaveNameInput();

        if (titleText != null)
            titleText.text = isSaveMode ? "SAVE GAME" : "LOAD GAME";
    }

    /// <summary>
    /// Chuyển đổi giữa chế độ Save và Load
    /// </summary>
    public void SetMode(bool saveMode)
    {
        isSaveMode = saveMode;
        if (titleText != null)
            titleText.text = isSaveMode ? "SAVE GAME" : "LOAD GAME";
        RefreshAllSlots();
    }

    /// <summary>
    /// Hiển thị menu Save/Load
    /// </summary>
    public void ShowSaveMenu(bool saveMode)
    {
        SetMode(saveMode);
        saveMenuPanel?.SetActive(true);
        RefreshAllSlots();
    }

    /// <summary>
    /// Ẩn menu Save/Load
    /// </summary>
    public void HideSaveMenu()
    {
        saveMenuPanel?.SetActive(false);
        HideSaveNameInput();
    }

    /// <summary>
    /// Được gọi từ SaveSlotUI khi click vào slot
    /// </summary>
    public void OnSlotClicked(int slotIndex)
    {
        selectedSlotIndex = slotIndex;

        if (isSaveMode)
        {
            // Chế độ Save: hiển thị input để nhập tên
            ShowSaveNameInput(slotIndex);
        }
        else
        {
            // Chế độ Load: load game trực tiếp
            LoadFromSlot(slotIndex);
        }
    }

    /// <summary>
    /// Được gọi từ SaveSlotUI khi click nút Delete
    /// </summary>
    public void OnDeleteSlot(int slotIndex)
    {
        if (saveService == null) return;

        if (saveService.HasSave(slotIndex))
        {
            saveService.DeleteSave(slotIndex);
            RefreshAllSlots();
            Debug.Log($"[SaveMenuUI] Deleted save slot {slotIndex}");
        }
    }

    private void ShowSaveNameInput(int slotIndex)
    {
        if (saveNameInputPanel == null || saveNameInput == null) return;

        saveNameInputPanel.SetActive(true);

        // Pre-fill với tên hiện tại hoặc mặc định
        SaveSlotData slotData = saveService.GetSaveSlotInfo(slotIndex);
        saveNameInput.text = slotData.isEmpty ? "New Save" : slotData.saveName;
        saveNameInput.Select();
        saveNameInput.ActivateInputField();
    }

    private void HideSaveNameInput()
    {
        saveNameInputPanel?.SetActive(false);
        selectedSlotIndex = -1;
    }

    private void OnConfirmSave()
    {
        if (saveService == null) return;
        if (selectedSlotIndex < 0 || selectedSlotIndex >= 4) return;

        string saveName = saveNameInput != null ? saveNameInput.text : "New Save";
        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = "Unnamed Save";
        }

        saveService.SaveGame(selectedSlotIndex, saveName);
        HideSaveNameInput();
        RefreshAllSlots();

        Debug.Log($"[SaveMenuUI] Saved to slot {selectedSlotIndex}: {saveName}");
    }

    private void OnCancelSave()
    {
        HideSaveNameInput();
    }

    private void LoadFromSlot(int slotIndex)
    {
        if (saveService == null) return;

        if (!saveService.HasSave(slotIndex))
        {
            Debug.LogWarning($"[SaveMenuUI] No save data in slot {slotIndex}");
            return;
        }

        if (saveService.LoadGame(slotIndex))
        {
            Debug.Log($"[SaveMenuUI] Loaded from slot {slotIndex}");
            HideSaveMenu();
        }
        else
        {
            Debug.LogError($"[SaveMenuUI] Failed to load from slot {slotIndex}");
        }
    }

    private void RefreshAllSlots()
    {
        if (saveService == null) return;

        SaveSlotData[] allSlots = saveService.GetAllSaveSlots();

        for (int i = 0; i < saveSlots.Length && i < allSlots.Length; i++)
        {
            if (saveSlots[i] != null)
            {
                saveSlots[i].UpdateSlotData(allSlots[i], isSaveMode);
            }
        }
    }
}