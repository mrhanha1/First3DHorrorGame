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
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;

    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    private ISettingsService settings;

    private void Awake()
    {
        if (!ServiceLocator.TryGet<ISettingsService>(out settings))
        {
            Debug.LogWarning("[SettingsUI] ISettingsService not available");
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
        antiAliasingDropdown?.onValueChanged.AddListener(OnAntiAliasingChanged);
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
        antiAliasingDropdown?.onValueChanged.RemoveListener(OnAntiAliasingChanged);
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
        if (antiAliasingDropdown != null)
        {
            // Convert AA value to dropdown index: 0->0, 2->1, 4->2, 8->3
            int index = settings.AntiAliasing == 0 ? 0 : (int)Mathf.Log(settings.AntiAliasing, 2);
            antiAliasingDropdown.value = index;
        }
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

    private void OnAntiAliasingChanged(int value)
    {
        settings?.SetAntiAliasing(value); // 0=Disabled, 1=2x, 2=4x, 3=8x
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