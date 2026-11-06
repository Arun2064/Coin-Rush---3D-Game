using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region UI References

    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [Header("Sliders")]
    public Slider menuMusicSlider;
    public Slider gameSoundSlider;
    public Slider sensitivitySlider;

    #endregion

    #region Constants

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";
    private const string MouseSensitivityKey = "MouseSensitivity";

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeSettings();
    }

    #endregion

    #region Initialization

    private void InitializeSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        LoadSavedSettings();
    }

    private void LoadSavedSettings()
    {
        if (menuMusicSlider != null)
        {
            menuMusicSlider.value =
                PlayerPrefs.GetFloat(
                    MusicVolumeKey,
                    1f
                );
        }

        if (gameSoundSlider != null)
        {
            gameSoundSlider.value =
                PlayerPrefs.GetFloat(
                    SfxVolumeKey,
                    1f
                );
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value =
                PlayerPrefs.GetFloat(
                    MouseSensitivityKey,
                    3f
                );
        }

        ApplyMenuMusicVolume(
            menuMusicSlider.value
        );

        ApplyGameSoundVolume(
            gameSoundSlider.value
        );
    }

    #endregion

    #region Panel Control

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    #endregion

    #region Audio Settings

    public void ApplyMenuMusicVolume(
        float value)
    {
        AudioManager.instance?.
            SetMenuMusicVolume(value);

        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            value
        );
    }

    public void ApplyGameSoundVolume(
        float value)
    {
        AudioManager.instance?.
            SetGameAudioVolume(value);

        PlayerPrefs.SetFloat(
            SfxVolumeKey,
            value
        );
    }

    #endregion

    #region Camera Settings

    public void ApplySensitivity(
        float value)
    {
        PlayerPrefs.SetFloat(
            MouseSensitivityKey,
            value
        );
    }

    #endregion
}