using TMPro;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    #region Coin Settings

    [Header("Coins")]
    public int targetCoins = 25;

    #endregion

    #region UI

    [Header("UI")]
    public TextMeshProUGUI coinText;

    #endregion

    #region Private Variables

    private int coins;

    #endregion

    #region Properties

    public int CoinCount => coins;

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeCoins();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin"))
            return;

        CollectCoin(other.gameObject);
    }

    #endregion

    #region Initialization

    private void InitializeCoins()
    {
        coins = 0;

        UpdateUI();
    }

    #endregion

    #region Coin Collection

    private void CollectCoin(GameObject coin)
    {
        AudioManager.instance?.PlayCoin();

        coins++;

        UpdateUI();

        Destroy(coin);

        if (coins >= targetCoins)
        {
            GameManager.instance?.WinGame();
        }
    }

    public void ResetCoins()
    {
        coins = 0;

        UpdateUI();
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        if (coinText == null)
            return;

        coinText.text =
            $"COINS: {coins}/{targetCoins}";
    }

    #endregion

    #region Public Getters

    public int GetCoinCount()
    {
        return coins;
    }

    #endregion
}