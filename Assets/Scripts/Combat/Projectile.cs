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
        Debug.Log(
            "Projectile hit: " +
            other.gameObject.name
        );

        if (owner != null &&
            other.transform.root == owner.root)
        {
            return;
        }

        TeamMember targetTeam =
            other.GetComponentInParent<TeamMember>();

        if (targetTeam != null)
        {
            Debug.Log(
                "Target team found: " +
                targetTeam.team
            );

            if (targetTeam.team == team)
            {
                Debug.Log("Friendly fire ignored.");
                return;
            }

            PlayerHealth playerHealth =
                other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                Debug.Log("PLAYER HIT!");

                playerHealth.TakeDamage(damage);

                Destroy(gameObject);

                return;
            }

            EnemyHealth enemyHealth =
                other.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Debug.Log("AI HIT!");

                enemyHealth.TakeDamage(damage);

                Destroy(gameObject);

                return;
            }
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}