using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Graphics")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

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
        musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
        brightnessSlider?.onValueChanged.AddListener(OnBrightnessChanged);
        qualityDropdown?.onValueChanged.AddListener(OnQualityChanged);
        sensitivitySlider?.onValueChanged.AddListener(OnSensitivityChanged);

        applyButton?.onClick.AddListener(OnApplyClicked);
        resetButton?.onClick.AddListener(OnResetClicked);
    }

    private void RemoveListeners()
    {
        masterVolumeSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        brightnessSlider?.onValueChanged.RemoveListener(OnBrightnessChanged);
        qualityDropdown?.onValueChanged.RemoveListener(OnQualityChanged);
        sensitivitySlider?.onValueChanged.RemoveListener(OnSensitivityChanged);

        applyButton?.onClick.RemoveListener(OnApplyClicked);
        resetButton?.onClick.RemoveListener(OnResetClicked);
    }

    private void LoadSettingsToUI()
    {
        if (settings == null) return;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = settings.MasterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = settings.MusicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = settings.SFXVolume;
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

    private void OnMusicVolumeChanged(float value)
    {
        settings?.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        settings?.SetSFXVolume(value);
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
}