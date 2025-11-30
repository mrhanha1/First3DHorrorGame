using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsService : ISettingsService
{
    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";
    private const string BRIGHTNESS_KEY = "Brightness";
    private const string QUALITY_KEY = "QualityLevel";
    private const string SENSITIVITY_KEY = "MouseSensitivity";

    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 0.8f;
    public float SFXVolume { get; private set; } = 1f;
    public float Brightness { get; private set; } = 1f;
    public int QualityLevel { get; private set; } = 2;
    public float MouseSensitivity { get; private set; } = 1f;

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
            audioMixer.SetFloat("MasterVolume", LinearToDecibel(MasterVolume));
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", LinearToDecibel(MusicVolume));
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", LinearToDecibel(SFXVolume));
    }

    public void SetBrightness(float value)
    {
        
        Brightness = Mathf.Clamp(value, 0f, 10f);
        RenderSettings.ambientIntensity = Brightness;
        RenderSettings.ambientLight = new Color(0.1f,0.1f,0.1f) * Brightness;// mau nen
        //Debug.Log("[SettingsService] Brightness set to: " + Brightness);
    }

    public void SetQualityLevel(int level)
    {
        QualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        //QualitySettings.SetQualityLevel(QualityLevel);
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

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, MasterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, MusicVolume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, SFXVolume);
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, Brightness);
        PlayerPrefs.SetInt(QUALITY_KEY, QualityLevel);
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, MouseSensitivity);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0.8f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f));
        SetBrightness(PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f));
        SetQualityLevel(PlayerPrefs.GetInt(QUALITY_KEY, 2));
        SetMouseSensitivity(PlayerPrefs.GetFloat(SENSITIVITY_KEY, 1f));
    }

    public void ResetToDefault()
    {
        SetMasterVolume(1f);
        SetMusicVolume(0.8f);
        SetSFXVolume(1f);
        SetBrightness(1f);
        SetQualityLevel(2);
        SetMouseSensitivity(1f);
        SaveSettings();
    }

    private float LinearToDecibel(float linear)
    {
        return linear > 0 ? 20f * Mathf.Log10(linear) : -80f;
    }
}