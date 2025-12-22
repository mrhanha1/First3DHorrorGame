public interface ISettingsService
{
    float MasterVolume { get; }
    float Brightness { get; }
    int QualityLevel { get; }
    float MouseSensitivity { get; }
    int AntiAliasing { get; }

    void SetMasterVolume(float value);
    void SetBrightness(float value);
    void SetQualityLevel(int level);
    void SetMouseSensitivity(float value);
    void SetAntiAliasing(int level);

    void SaveSettings();
    void LoadSettings();
    void ResetToDefault();
}