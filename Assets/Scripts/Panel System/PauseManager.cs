using UnityEngine;

public class PauseManager : MonoBehaviour
{
    #region Singleton

    public static PauseManager instance;

    #endregion

    #region UI References

    [Header("UI")]
    public GameObject pausePanel;

    #endregion

    #region Private Variables

    private bool isPaused;

    #endregion

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePauseMenu();
    }

    private void Update()
    {
        HandlePauseInput();
    }

    #endregion

    #region Initialization

    private void InitializePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        isPaused = false;
    }

    #endregion

    #region Input

    private void HandlePauseInput()
    {
        if (GameManager.instance == null)
            return;

        if (GameManager.instance.IsGameEnded())
            return;

        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    #endregion

    #region Pause Control

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        AudioManager.instance?.PlayIdle();
    }

    #endregion

    #region Button Actions

    public void RestartGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        GameManager.instance.RestartGame();
    }

    public void MainMenu()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        AudioManager.instance?.StopAllAudio();

        GameManager.instance.LoadMainMenu();
    }

    #endregion

    #region Utility

    public bool IsPaused()
    {
        return isPaused;
    }

    #endregion
}