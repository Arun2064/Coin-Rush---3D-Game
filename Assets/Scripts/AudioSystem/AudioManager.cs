using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager instance;

    #endregion

    #region Audio Sources

    [Header("Audio Sources")]
    public AudioSource menuMusicSource;
    public AudioSource gameAudioSource;
    public AudioSource sfxSource;

    #endregion

    #region Audio Clips

    [Header("Menu")]
    public AudioClip menuMusic;

    [Header("Gameplay Audio")]
    public AudioClip idleClip;
    public AudioClip walkClip;

    [Header("Sound Effects")]
    public AudioClip coinClip;
    public AudioClip hurtClip;
    public AudioClip winClip;
    public AudioClip gameOverClip;
    public AudioClip buttonClickClip;

    #endregion

    #region Private Variables

    private bool isWalking;

    #endregion

    #region Unity Events

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSavedVolumes();
    }

    #endregion

    #region Initialization

    private void LoadSavedVolumes()
    {
        SetMenuMusicVolume(
            PlayerPrefs.GetFloat("MusicVolume", 1f)
        );

        SetGameAudioVolume(
            PlayerPrefs.GetFloat("SFXVolume", 1f)
        );
    }

    #endregion

    #region Menu Audio

    public void PlayMenuMusic()
    {
        if (menuMusicSource == null || menuMusic == null)
            return;

        if (
            menuMusicSource.clip == menuMusic &&
            menuMusicSource.isPlaying
        )
        {
            return;
        }

        menuMusicSource.clip = menuMusic;
        menuMusicSource.loop = true;
        menuMusicSource.Play();
    }

    public void StopMenuMusic()
    {
        if (menuMusicSource == null)
            return;

        menuMusicSource.Stop();
    }

    #endregion

    #region Gameplay Audio

    public void PlayIdle()
    {
        if (gameAudioSource == null || idleClip == null)
            return;

        if (
            gameAudioSource.clip == idleClip &&
            gameAudioSource.isPlaying
        )
        {
            return;
        }

        gameAudioSource.clip = idleClip;
        gameAudioSource.loop = true;
        gameAudioSource.Play();

        isWalking = false;
    }

    public void PlayWalk()
    {
        if (gameAudioSource == null || walkClip == null)
            return;

        if (isWalking)
            return;

        gameAudioSource.clip = walkClip;
        gameAudioSource.loop = true;
        gameAudioSource.Play();

        isWalking = true;
    }

    public void StopWalk()
    {
        if (!isWalking)
            return;

        PlayIdle();
    }

    public void StopGameplayAudio()
    {
        if (gameAudioSource != null)
        {
            gameAudioSource.Stop();
        }

        isWalking = false;
    }

    #endregion

    #region Sound Effects

    public void PlayCoin()
    {
        PlaySFX(coinClip);
    }

    public void PlayHurt()
    {
        PlaySFX(hurtClip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayWin()
    {
        if (winClip == null)
            return;

        AudioSource.PlayClipAtPoint(
            winClip,
            Camera.main.transform.position,
            sfxSource != null ? sfxSource.volume : 1f
        );
    }

    public void PlayGameOver()
    {
        if (gameOverClip == null)
            return;

        AudioSource.PlayClipAtPoint(
            gameOverClip,
            Camera.main.transform.position,
            sfxSource != null ? sfxSource.volume : 1f
        );
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    #endregion

    #region Volume Controls

    public void SetMenuMusicVolume(float value)
    {
        if (menuMusicSource != null)
        {
            menuMusicSource.volume = value;
        }

        PlayerPrefs.SetFloat(
            "MusicVolume",
            value
        );
    }

    public void SetGameAudioVolume(float value)
    {
        if (gameAudioSource != null)
        {
            gameAudioSource.volume = value;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }

        PlayerPrefs.SetFloat(
            "SFXVolume",
            value
        );
    }

    #endregion

    #region Global Audio Controls

    public void StopAllAudio()
    {
        menuMusicSource?.Stop();
        gameAudioSource?.Stop();
        sfxSource?.Stop();

        isWalking = false;
    }

    #endregion
}