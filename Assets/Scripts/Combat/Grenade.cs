using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseTime = 3f;

    [Header("Damage Zones")]
    public float closeRange = 1f;
    public float mediumRange = 3f;
    public float farRange = 7f;

    [Header("Close Damage")]
    public float closeDamage = 300f;

    [Header("Medium Damage")]
    public float mediumDamageMin = 125f;
    public float mediumDamageMax = 175f;

    [Header("Far Damage")]
    public float farDamageMin = 45f;
    public float farDamageMax = 65f;

    [HideInInspector]
    public Team team;

    [HideInInspector]
    public Transform owner;

    private bool hasExploded = false;

    private void Start()
    {
        Invoke(
            nameof(Explode),
            fuseTime
        );
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        Debug.Log("GRENADE EXPLODED");

        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                farRange
            );

        HashSet<GameObject> damagedObjects =
            new HashSet<GameObject>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            // Ignore grenades completely.
            if (hit.GetComponentInParent<Grenade>() != null)
            {
                continue;
            }

            // =====================================
            // PLAYER
            // =====================================

            PlayerHealth playerHealth =
                hit.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                GameObject target =
                    playerHealth.gameObject;

                if (!damagedObjects.Contains(target))
                {
                    TeamMember targetTeam =
                        playerHealth.GetComponentInParent<TeamMember>();

                    if (targetTeam != null &&
                        targetTeam.team != team)
                    {
                        float distance =
                            GetDistanceToTarget(hit);

                        float damage =
                            GetDamageForDistance(
                                distance
                            );

                        if (damage > 0f)
                        {
                            Debug.Log(
                                "Grenade hit player at " +
                                distance.ToString("F1") +
                                " meters for " +
                                damage.ToString("F0") +
                                " damage."
                            );

                            playerHealth.TakeDamage(
                                damage
                            );

                            damagedObjects.Add(
                                target
                            );
                        }
                    }
                }
            }

            // =====================================
            // AI
            // =====================================

            EnemyHealth enemyHealth =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                GameObject target =
                    enemyHealth.gameObject;

                if (!damagedObjects.Contains(target))
                {
                    TeamMember targetTeam =
                        enemyHealth.GetComponentInParent<TeamMember>();

                    if (targetTeam != null &&
                        targetTeam.team != team)
                    {
                        float distance =
                            GetDistanceToTarget(hit);

                        float damage =
                            GetDamageForDistance(
                                distance
                            );

                        if (damage > 0f)
                        {
                            Debug.Log(
                                "Grenade hit AI at " +
                                distance.ToString("F1") +
                                " meters for " +
                                damage.ToString("F0") +
                                " damage."
                            );

                            enemyHealth.TakeDamage(
                                damage
                            );

                            damagedObjects.Add(
                                target
                            );
                        }
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    // =========================================
    // DISTANCE
    // =========================================

    private float GetDistanceToTarget(
        Collider targetCollider
    )
    {
        Vector3 closestPoint =
            targetCollider.ClosestPoint(
                transform.position
            );

        return Vector3.Distance(
            transform.position,
            closestPoint
        );
    }

    // =========================================
    // DAMAGE ZONES
    // =========================================

    private float GetDamageForDistance(
        float distance
    )
    {
        // Direct / very close hit.
        // Always lethal damage.
        if (distance <= closeRange)
        {
            return closeDamage;
        }

        // Medium explosion range.
        if (distance <= mediumRange)
        {
            return Random.Range(
                mediumDamageMin,
                mediumDamageMax
            );
        }

        // Outer explosion range.
        if (distance <= farRange)
        {
            return Random.Range(
                farDamageMin,
                farDamageMax
            );
        }

        return 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            closeRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            mediumRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            farRange
        );
    }
}