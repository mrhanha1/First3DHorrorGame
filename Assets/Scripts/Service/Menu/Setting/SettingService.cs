using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsService : ISettingsService
{
    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string BRIGHTNESS_KEY = "Brightness";
    private const string QUALITY_KEY = "QualityLevel";
    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string AA_KEY = "AntiAliasing";

    public float MasterVolume { get; private set; } = 1f;
    public float Brightness { get; private set; } = 1f;
    public int QualityLevel { get; private set; } = 2;
    public float MouseSensitivity { get; private set; } = 1f;
    public int AntiAliasing { get; private set; } = 2;

    private AudioMixer audioMixer;

    public SettingsService(AudioMixer mixer = null)
    {
        audioMixer = mixer;
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
        {
            if (MasterVolume <= 0.0001f)
                audioMixer.SetFloat("MasterVolume", -80f);
            else
                audioMixer.SetFloat("MasterVolume", LinearToDecibel(MasterVolume));
        }
    }

    public void SetBrightness(float value)
    {
        Brightness = Mathf.Clamp(value, 0f, 10f);
        RenderSettings.ambientIntensity = Brightness;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f) * Brightness;
    }

    public void SetQualityLevel(int level)
    {
        QualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(QualityLevel);
    }

    public void SetMouseSensitivity(float value)
    {
        var controller = GameObject.FindObjectOfType<FirstPersonController>();
        if (controller != null)
        {
            MouseSensitivity = Mathf.Clamp(value, 0.5f, 8f);
            controller.RotationSpeed = MouseSensitivity;
        }
        else
        {
            Debug.LogError("[SettingsService] FirstPersonController not found in scene.");
        }
    }

    public void SetAntiAliasing(int level)
    {
        // level: 0 = Disabled, 1 = 2x, 2 = 4x, 3 = 8x
        int aaValue = level == 0 ? 0 : (int)Mathf.Pow(2, level);
        AntiAliasing = aaValue;
        QualitySettings.antiAliasing = aaValue;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, MasterVolume);
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, Brightness);
        PlayerPrefs.SetInt(QUALITY_KEY, QualityLevel);
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, MouseSensitivity);
        PlayerPrefs.SetInt(AA_KEY, AntiAliasing);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f));
        SetBrightness(PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f));
        SetQualityLevel(PlayerPrefs.GetInt(QUALITY_KEY, 2));
        SetMouseSensitivity(PlayerPrefs.GetFloat(SENSITIVITY_KEY, 1f));
        SetAntiAliasing(PlayerPrefs.GetInt(AA_KEY, 1)); // Default 2x
    }

    public void ResetToDefault()
    {
        SetMasterVolume(1f);
        SetBrightness(1f);
        SetQualityLevel(2);
        SetMouseSensitivity(1f);
        SetAntiAliasing(1); // 2x
        SaveSettings();
    }

    private float LinearToDecibel(float linear)
    {
        return linear > 0 ? 20f * Mathf.Log10(linear) : -80f;
    }
}