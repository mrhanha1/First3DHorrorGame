using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private SaveSlotUI[] saveSlots;
    [SerializeField] private GameObject saveNameInputPanel;
    [SerializeField] private TMP_InputField saveNameInput;
    [SerializeField] private Button confirmSaveButton;
    [SerializeField] private Button cancelSaveButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private ISaveService saveService;
    private bool isSaveMode = true;
    private int selectedSlotIndex = -1;

    private void Awake()
    {
        if (ServiceLocator.TryGet<ISaveService>(out ISaveService service))
        {
            saveService = service;
        }
        else
        {
            Debug.LogError("[SaveMenuUI] SaveService not found in ServiceLocator!");
        }

        if (confirmSaveButton != null)
            confirmSaveButton.onClick.AddListener(OnConfirmSave);

        if (cancelSaveButton != null)
            cancelSaveButton.onClick.AddListener(OnCancelSave);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnEnable()
    {
        RefreshAllSlots();
        HideSaveNameInput();
        UpdateTitle();
    }

    public void SetMode(bool saveMode)
    {
        isSaveMode = saveMode;
        UpdateTitle();
        RefreshAllSlots();
    }

    public void OnSlotClicked(int slotIndex)
    {
        selectedSlotIndex = slotIndex;

        if (isSaveMode)
        {
            ShowSaveNameInput(slotIndex);
        }
        else
        {
            LoadFromSlot(slotIndex);
        }
    }

    public void OnDeleteSlot(int slotIndex)
    {
        if (saveService == null) return;

        if (saveService.HasSave(slotIndex))
        {
            saveService.DeleteSave(slotIndex);
            RefreshAllSlots();
        }
    }

    private void UpdateTitle()
    {
        if (titleText != null)
            titleText.text = isSaveMode ? "SAVE GAME" : "LOAD GAME";
    }

    private void ShowSaveNameInput(int slotIndex)
    {
        if (saveNameInputPanel == null || saveNameInput == null) return;

        if (saveService == null)
        {
            Debug.LogError("[SaveMenuUI] SaveService not found in ServiceLocator!");
            return;
        }
        saveNameInputPanel.SetActive(true);

        SaveSlotData slotData = saveService.GetSaveSlotInfo(slotIndex);
        saveNameInput.text = slotData.isEmpty ? "Bản lưu mới" : slotData.saveName;
        saveNameInput.Select();
        saveNameInput.ActivateInputField();
        Debug.Log($"[SaveMenuUI] Showing save name input for slot {slotIndex}");
    }

    private void HideSaveNameInput()
    {
        saveNameInputPanel?.SetActive(false);
        selectedSlotIndex = -1;
    }

    private void OnConfirmSave()
    {
        if (saveService == null) return;

        int maxSlots = saveService.GetMaxSlots();
        if (selectedSlotIndex < 0 || selectedSlotIndex >= maxSlots) return;

        string saveName = saveNameInput != null ? saveNameInput.text : "New Save";
        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = "Unnamed Save";
        }

        saveService.SaveGame(selectedSlotIndex, saveName);
        HideSaveNameInput();
        RefreshAllSlots();
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
            // Đóng menu sau khi load thành công
            MenuManager.Instance?.CloseAll();
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

    private void OnBackClicked()
    {
        MenuManager.Instance?.OnBackClicked();
    }
}