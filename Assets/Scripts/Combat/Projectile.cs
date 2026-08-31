using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileTeam
    {
        Player,
        Enemy
    }

    [Header("Projectile Stats")]
    public ProjectileTeam team;

    public float speed = 40f;
    public float damage = 25f;
    public float lifetime = 10f;

    [HideInInspector]
    public Transform owner;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore whoever fired this bullet
        if (owner != null && other.transform.root == owner.root)
        {
            return;
        }

        // Player bullet hits enemy
        if (team == ProjectileTeam.Player)
        {
            EnemyHealth enemyHealth =
                other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Enemy bullet hits player
        if (team == ProjectileTeam.Enemy)
        {
            PlayerHealth playerHealth =
                other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Hit environment
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}