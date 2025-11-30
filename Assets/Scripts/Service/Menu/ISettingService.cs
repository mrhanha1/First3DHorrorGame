public interface ISettingsService
{
    float MasterVolume { get; }
    float MusicVolume { get; }
    float SFXVolume { get; }
    float Brightness { get; }
    int QualityLevel { get; }
    float MouseSensitivity { get; }

    void SetMasterVolume(float value);
    void SetMusicVolume(float value);
    void SetSFXVolume(float value);
    void SetBrightness(float value);
    void SetQualityLevel(int level);
    void SetMouseSensitivity(float value);

    void SaveSettings();
    void LoadSettings();
    void ResetToDefault();
}