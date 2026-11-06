using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerHealth : MonoBehaviour
{
    #region Health Settings

    [Header("Health")]
    public Image healthFill;
    public float maxHealth = 100f;

    #endregion

    #region Damage Settings

    [Header("Damage")]
    public float damageOnHit = 20f;

    #endregion

    #region Knockback Settings

    [Header("Knockback")]
    public float knockbackStrength = 8f;
    public float knockbackDuration = 0.4f;

    #endregion

    #region Private Variables

    private float currentHealth;

    private CharacterController controller;

    private bool isKnockedBack;

    #endregion

    #region Unity Events

    private void Start()
    {
        InitializeHealth();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fire"))
            return;

        HandleFireCollision(other);
    }

    #endregion

    #region Initialization

    private void InitializeHealth()
    {
        controller =
            GetComponent<CharacterController>();

        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    #endregion

    #region Damage System

    private void HandleFireCollision(
        Collider fire)
    {
        TakeDamage(damageOnHit);

        GameManager.instance?.AddPenaltyTime(2f);

        AudioManager.instance?.PlayHurt();

        Vector3 knockbackDirection =
            (
                transform.position -
                fire.transform.position
            ).normalized;

        knockbackDirection.y = 0f;

        if (!isKnockedBack)
        {
            StartCoroutine(
                Knockback(
                    knockbackDirection,
                    knockbackStrength,
                    knockbackDuration
                )
            );
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            GameManager.instance?.TriggerGameOver();
        }
    }

    #endregion

    #region Health Management

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    #endregion

    #region UI

    private void UpdateHealthUI()
    {
        if (healthFill == null)
            return;

        healthFill.fillAmount =
            currentHealth / maxHealth;
    }

    #endregion

    #region Knockback

    private IEnumerator Knockback(
        Vector3 direction,
        float strength,
        float duration)
    {
        isKnockedBack = true;

        float timer = 0f;

        while (timer < duration)
        {
            controller.Move(
                direction *
                strength *
                (1f - timer / duration) *
                Time.deltaTime
            );

            timer += Time.deltaTime;

            yield return null;
        }

        isKnockedBack = false;
    }

    #endregion
}