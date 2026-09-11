using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
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
            spawnMenu.StartDeathScreen(gameObject);
        }
    }
}