using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public Team team;

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
            rb.linearVelocity =
                transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore whoever fired the projectile
        if (owner != null &&
            other.transform.root == owner.root)
        {
            return;
        }

        TeamMember targetTeam =
            other.GetComponentInParent<TeamMember>();

        // Hit a character
        if (targetTeam != null)
        {
            // Friendly fire off
            if (targetTeam.team == team)
            {
                return;
            }

            EnemyHealth enemyHealth =
                other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

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