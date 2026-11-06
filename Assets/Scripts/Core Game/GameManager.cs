using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    #endregion

    #region References

    [Header("Player")]
    public Transform player;
    public Transform fixedSpawnPoint;

    [Header("HUD")]
    public TMP_Text timerText;

    #endregion

    #region Win UI

    [Header("Win Panel")]
    public GameObject winPanel;
    public TMP_Text winTimeText;
    public TMP_Text winCoinText;
    public GameObject highScoreText;

    #endregion

    #region Game Over UI

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverTimeText;
    public TMP_Text gameOverCoinText;

    #endregion

    #region Coin Settings

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int coinCount = 25;
    public Vector3 spawnArea =
        new Vector3(20f, 1f, 20f);

    #endregion

    #region Private Variables

    private float elapsedTime;
    private bool gameEnded;

    private readonly List<GameObject> spawnedCoins =
        new();

    private CharacterController characterController;
    private ThirdPersonController playerController;
    private PlayerHealth playerHealth;
    private CoinCollector coinCollector;

    #endregion

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        UpdateTimer();
    }

    #endregion

    #region Initialization

    private void InitializeGame()
    {
        AudioManager.instance?.StopMenuMusic();
        AudioManager.instance?.PlayIdle();

        Time.timeScale = 1f;

        elapsedTime = 0f;
        gameEnded = false;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (highScoreText != null)
            highScoreText.SetActive(false);

        CacheReferences();

        SpawnCoins();
    }

    private void CacheReferences()
    {
        characterController =
            player.GetComponent<CharacterController>();

        playerController =
            player.GetComponent<ThirdPersonController>();

        playerHealth =
            player.GetComponent<PlayerHealth>();

        coinCollector =
            player.GetComponent<CoinCollector>();
    }

    #endregion

    #region Timer

    private void UpdateTimer()
    {
        if (gameEnded)
            return;

        elapsedTime += Time.deltaTime;

        if (timerText != null)
        {
            timerText.text =
                $"TIME: {elapsedTime:F2}s";
        }
    }

    public void AddPenaltyTime(float seconds)
    {
        elapsedTime += seconds;
    }

    #endregion

    #region State

    public bool IsGameEnded()
    {
        return gameEnded;
    }

    #endregion

    #region Win

    public void WinGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        AudioManager.instance?.StopGameplayAudio();
        AudioManager.instance?.PlayWin();

        Time.timeScale = 0f;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (winCoinText != null)
        {
            winCoinText.text =
                $"COINS: {coinCollector.CoinCount}/{coinCount}";
        }

        if (winTimeText != null)
        {
            winTimeText.text =
                $"TIME: {elapsedTime:F2}s";
        }

        bool isHighScore =
            HighScoreManager.IsHighScore(
                elapsedTime
            );

        if (highScoreText != null)
        {
            highScoreText.SetActive(
                isHighScore
            );
        }

        HighScoreManager.SaveScore(
            elapsedTime
        );

        winPanel.SetActive(true);

        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }

    #endregion

    #region Game Over

    public void TriggerGameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        AudioManager.instance?.StopGameplayAudio();
        AudioManager.instance?.PlayGameOver();

        Time.timeScale = 0f;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (gameOverCoinText != null)
        {
            gameOverCoinText.text =
                $"COINS: {coinCollector.CoinCount}/{coinCount}";
        }

        if (gameOverTimeText != null)
        {
            gameOverTimeText.text =
                $"TIME: {elapsedTime:F2}s";
        }

        gameOverPanel.SetActive(true);

        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }

    #endregion

    #region Restart

    public void RestartGame()
    {
        Time.timeScale = 1f;

        elapsedTime = 0f;
        gameEnded = false;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (highScoreText != null)
        {
            highScoreText.SetActive(false);
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        player.position =
            fixedSpawnPoint.position;

        player.rotation =
            fixedSpawnPoint.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        if (playerController != null)
        {
            playerController.canMove = true;
        }

        playerHealth?.ResetHealth();
        coinCollector?.ResetCoins();

        ClearCoins();
        SpawnCoins();

        AudioManager.instance?.PlayIdle();
    }

    #endregion

    #region Scene Management

    public void LoadMainMenu()
    {
        AudioManager.instance?.StopAllAudio();

        Time.timeScale = 1f;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        SceneManager.LoadScene(
            "MainMenu"
        );
    }

    #endregion

    #region Coin Spawning

    private void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomPosition =
                new Vector3(
                    Random.Range(
                        -spawnArea.x / 2f,
                        spawnArea.x / 2f
                    ),
                    spawnArea.y,
                    Random.Range(
                        -spawnArea.z / 2f,
                        spawnArea.z / 2f
                    )
                );

            GameObject coin =
                Instantiate(
                    coinPrefab,
                    randomPosition,
                    Quaternion.identity
                );

            spawnedCoins.Add(coin);
        }
    }

    private void ClearCoins()
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
            {
                Destroy(coin);
            }
        }

        spawnedCoins.Clear();
    }

    #endregion
}