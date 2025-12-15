using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Graphics")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    private ISettingsService settings;

    private void Awake()
    {
        settings = ServiceLocator.Get<ISettingsService>();
        if (settings == null)
        {
            Debug.LogError("[SettingsUI] ISettingsService not found!");
        }
    }

    private void OnEnable()
    {
        LoadSettingsToUI();
        SetupListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void SetupListeners()
    {
        masterVolumeSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
        brightnessSlider?.onValueChanged.AddListener(OnBrightnessChanged);
        qualityDropdown?.onValueChanged.AddListener(OnQualityChanged);
        sensitivitySlider?.onValueChanged.AddListener(OnSensitivityChanged);

        applyButton?.onClick.AddListener(OnApplyClicked);
        resetButton?.onClick.AddListener(OnResetClicked);
        backButton?.onClick.AddListener(OnBackClicked);
    }

    private void RemoveListeners()
    {
        masterVolumeSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        brightnessSlider?.onValueChanged.RemoveListener(OnBrightnessChanged);
        qualityDropdown?.onValueChanged.RemoveListener(OnQualityChanged);
        sensitivitySlider?.onValueChanged.RemoveListener(OnSensitivityChanged);

        applyButton?.onClick.RemoveListener(OnApplyClicked);
        resetButton?.onClick.RemoveListener(OnResetClicked);
        backButton?.onClick.RemoveListener(OnBackClicked);
    }

    private void LoadSettingsToUI()
    {
        if (settings == null) return;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = settings.MasterVolume;
        if (brightnessSlider != null)
            brightnessSlider.value = settings.Brightness;
        if (qualityDropdown != null)
            qualityDropdown.value = settings.QualityLevel;
        if (sensitivitySlider != null)
            sensitivitySlider.value = settings.MouseSensitivity;
    }

    private void OnMasterVolumeChanged(float value)
    {
        settings?.SetMasterVolume(value);
    }

    private void OnBrightnessChanged(float value)
    {
        settings?.SetBrightness(value);
    }

    private void OnQualityChanged(int value)
    {
        settings?.SetQualityLevel(value);
    }

    private void OnSensitivityChanged(float value)
    {
        settings?.SetMouseSensitivity(value);
    }

    private void OnApplyClicked()
    {
        settings?.SaveSettings();
    }

    private void OnResetClicked()
    {
        settings?.ResetToDefault();
        LoadSettingsToUI();
    }

    private void OnBackClicked()
    {
        MenuManager.Instance?.OnBackClicked();
    }
}