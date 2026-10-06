using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("UI")]
    public TMP_Text healthText;

    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {

        currentHealth = maxHealth;

        PlayerHealthUI healthUI =
            FindFirstObjectByType<PlayerHealthUI>();

        if (healthUI != null)
        {
            healthText =
                healthUI.healthText;
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Player HP: " +
            currentHealth
        );

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText == null)
            return;

        healthText.text =
            "HEALTH: " +
            Mathf.CeilToInt(currentHealth) +
            " / " +
            Mathf.CeilToInt(maxHealth);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("PLAYER DIED");

        ThirdpersonPlayer playerController =
            GetComponent<ThirdpersonPlayer>();

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        CharacterController characterController =
            GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        SpawnMenu spawnMenu =
            FindFirstObjectByType<SpawnMenu>();

        if (spawnMenu != null)
        {
            spawnMenu.StartDeathScreen(
                gameObject
            );
        }
    }
}