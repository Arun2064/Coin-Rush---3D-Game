using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    #region UI Panels

    [Header("Panels")]
    public GameObject backgroundPanel;
    public GameObject howToPlayPanel;
    public GameObject bestScorePanel;

    #endregion

    #region High Score UI

    [Header("High Score UI")]
    public TMP_Text scoresText;

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeMainMenu();
    }

    #endregion

    #region Initialization

    private void InitializeMainMenu()
    {
        ShowMainMenu();

        AudioManager.instance?.PlayMenuMusic();

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    #endregion

    #region Play

    public void PlayGame()
    {
        AudioManager.instance?.StopMenuMusic();

        SceneManager.LoadScene(
            "GAME Scene"
        );
    }

    #endregion

    #region Navigation

    public void OpenHowToPlay()
    {
        AudioManager.instance?.PlayButtonClick();

        backgroundPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        bestScorePanel.SetActive(false);
    }

    public void OpenHighScores()
    {
        AudioManager.instance?.PlayButtonClick();

        backgroundPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        bestScorePanel.SetActive(true);

        LoadScores();
    }

    public void BackToMenu()
    {
        AudioManager.instance?.PlayButtonClick();

        AudioManager.instance?.StopGameplayAudio();

        ShowMainMenu();

        AudioManager.instance?.PlayMenuMusic();
    }

    private void ShowMainMenu()
    {
        backgroundPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
        bestScorePanel.SetActive(false);
    }

    #endregion

    #region High Scores

    private void LoadScores()
    {
        List<float> scores =
            HighScoreManager.GetScores();

        if (scores.Count == 0)
        {
            scoresText.text =
                "No Scores Yet";

            return;
        }

        string displayText = "";

        for (int i = 0; i < scores.Count; i++)
        {
            displayText +=
                $"{i + 1}. {scores[i]:F2} sec\n";
        }

        scoresText.text = displayText;
    }

    public void ClearScores()
    {
        AudioManager.instance?.PlayButtonClick();

        HighScoreManager.ClearScores();

        scoresText.text =
            "No Scores Yet";
    }

    #endregion

    #region Quit

    public void QuitGame()
    {
        AudioManager.instance?.PlayButtonClick();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}