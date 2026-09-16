using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    [Header("Movement")]
    public float shootRange = 10f;
    public float rotationSpeed = 8f;

    [Header("Weapon")]
    public float fireRate = 1f;
    public float bulletSpeed = 20f;
    public float bulletDamage = 20f;
    public float bulletLifetime = 10f;

    [Header("Combat Targeting")]
    public float enemyDetectionRange = 15f;
    public float targetSearchInterval = 0.5f;

    [Header("Objectives")]
    public float objectiveStopDistance = 2f;

    private TeamMember myTeam;

    private Transform currentEnemy;
    private CommandPost currentObjective;

    private float nextFireTime;
    private float nextTargetSearchTime;

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        myTeam = GetComponent<TeamMember>();

        FindEnemy();
        FindObjective();
    }

    private void Update()
    {
        if (myTeam == null)
            return;

        // Periodically update combat target
        // and command-post objective.
        if (Time.time >= nextTargetSearchTime)
        {
            FindEnemy();

            if (currentObjective == null ||
                currentObjective.IsOwnedBy(myTeam.team))
            {
                FindObjective();
            }

            nextTargetSearchTime =
                Time.time + targetSearchInterval;
        }

        // Priority 1:
        // Fight nearby enemies.
        if (currentEnemy != null)
        {
            HandleEnemy();
            return;
        }

        // Priority 2:
        // Capture command posts.
        if (currentObjective != null)
        {
            HandleObjective();
            return;
        }

        // Nothing to do.
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private void FindEnemy()
    {
        TeamMember[] allTeamMembers =
            FindObjectsByType<TeamMember>(
                FindObjectsSortMode.None
            );

        float closestDistance =
            enemyDetectionRange;

        Transform closestTarget =
            null;

        foreach (TeamMember member in allTeamMembers)
        {
            if (member == null)
                continue;

            if (member == myTeam)
                continue;

            if (member.team == myTeam.team)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    member.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget =
                    member.transform;
            }
        }

        currentEnemy =
            closestTarget;
    }

    private void FindObjective()
    {
        CommandPost[] commandPosts =
            FindObjectsByType<CommandPost>(
                FindObjectsSortMode.None
            );

        float closestDistance =
            Mathf.Infinity;

        CommandPost closestPost =
            null;

        foreach (CommandPost post in commandPosts)
        {
            if (post == null)
                continue;

            // Ignore posts already owned
            // by our faction.
            if (post.IsOwnedBy(myTeam.team))
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    post.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closestPost =
                    post;
            }
        }

        currentObjective =
            closestPost;
    }

    private void HandleEnemy()
    {
        if (currentEnemy == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                currentEnemy.position
            );

        if (distance > shootRange)
        {
            MoveTo(
                currentEnemy.position
            );
        }
        else
        {
            StopAndShoot();
        }
    }

    private void HandleObjective()
    {
        if (currentObjective == null)
            return;

        // Objective was captured by our team.
        // Find another one.
        if (currentObjective.IsOwnedBy(myTeam.team))
        {
            currentObjective = null;

            FindObjective();

            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                currentObjective.transform.position
            );

        // Move into the command post.
        if (distance > objectiveStopDistance)
        {
            MoveTo(
                currentObjective.transform.position
            );
        }
        else
        {
            // Stay inside the capture area.
            if (agent != null)
            {
                agent.isStopped = true;
            }
        }
    }

    private void MoveTo(Vector3 position)
    {
        if (agent == null)
            return;

        agent.isStopped = false;

        agent.SetDestination(
            position
        );
    }

    private void StopAndShoot()
    {
        if (currentEnemy == null)
            return;

        if (agent != null)
        {
            agent.isStopped = true;
        }

        Vector3 direction =
            currentEnemy.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime =
                Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (shootPoint == null ||
            bulletPrefab == null)
        {
            return;
        }

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                shootPoint.position,
                shootPoint.rotation
            );

        Projectile projectile =
            bullet.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.team =
                myTeam.team;

            projectile.owner =
                transform.root;

            projectile.speed =
                bulletSpeed;

            projectile.damage =
                bulletDamage;

            projectile.lifetime =
                bulletLifetime;
        }
    }
}