using System.Collections.Generic;
using UnityEngine;

public static class HighScoreManager
{
    #region Constants

    private const int MaxScores = 5;

    private const string ScoreCountKey =
        "ScoreCount";

    private const string ScoreKeyPrefix =
        "Score_";

    #endregion

    #region Save Scores

    public static void SaveScore(float time)
    {
        List<float> scores =
            GetScores();

        scores.Add(time);

        scores.Sort();

        TrimScores(scores);

        SaveScoresToPrefs(scores);
    }

    #endregion

    #region Load Scores

    public static List<float> GetScores()
    {
        List<float> scores =
            new List<float>();

        int scoreCount =
            PlayerPrefs.GetInt(
                ScoreCountKey,
                0
            );

        for (int i = 0; i < scoreCount; i++)
        {
            scores.Add(
                PlayerPrefs.GetFloat(
                    ScoreKeyPrefix + i
                )
            );
        }

        return scores;
    }

    #endregion

    #region High Score Check

    public static bool IsHighScore(float time)
    {
        List<float> scores =
            GetScores();

        if (scores.Count < MaxScores)
            return true;

        return time <
               scores[scores.Count - 1];
    }

    #endregion

    #region Clear Scores

    public static void ClearScores()
    {
        int scoreCount =
            PlayerPrefs.GetInt(
                ScoreCountKey,
                0
            );

        for (int i = 0; i < scoreCount; i++)
        {
            PlayerPrefs.DeleteKey(
                ScoreKeyPrefix + i
            );
        }

        PlayerPrefs.DeleteKey(
            ScoreCountKey
        );

        PlayerPrefs.Save();
    }

    #endregion

    #region Helpers

    private static void TrimScores(
        List<float> scores)
    {
        if (scores.Count <= MaxScores)
            return;

        scores.RemoveRange(
            MaxScores,
            scores.Count - MaxScores
        );
    }

    private static void SaveScoresToPrefs(
        List<float> scores)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetFloat(
                ScoreKeyPrefix + i,
                scores[i]
            );
        }

        PlayerPrefs.SetInt(
            ScoreCountKey,
            scores.Count
        );

        PlayerPrefs.Save();
    }

    #endregion
}