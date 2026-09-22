using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;

    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    public bool IsInjured
    {
        get { return currentHealth < maxHealth; }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            gameObject.name +
            " HP: " +
            currentHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            gameObject.name +
            " HEALED! HP: " +
            currentHealth
        );
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}