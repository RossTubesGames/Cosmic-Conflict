using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIClass
    {
        Assault,
        Heavy,
        Medic
    }

    public enum SquadOrder
    {
        None,
        Follow,
        Hold
    }

    [Header("Class")]
    public AIClass aiClass = AIClass.Assault;

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

    [Header("Medic")]
    public float medicSearchRange = 20f;
    public float healRange = 2.5f;
    public float healAmount = 20f;
    public float healInterval = 1f;

    [Header("Squad")]
    public SquadOrder squadOrder =
        SquadOrder.None;

    public float squadStopDistance = 1f;

    private TeamMember myTeam;

    private Transform currentEnemy;
    private CommandPost currentObjective;

    private EnemyHealth injuredAlly;

    private Vector3 formationPosition;
    private Vector3 holdPosition;

    private float nextFireTime;
    private float nextHealTime;
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

        if (aiClass == AIClass.Medic)
        {
            FindInjuredAlly();
        }
    }

    private void Update()
    {
        if (myTeam == null)
            return;

        if (Time.time >= nextTargetSearchTime)
        {
            FindEnemy();

            if (currentObjective == null ||
                currentObjective.IsOwnedBy(myTeam.team))
            {
                FindObjective();
            }

            if (aiClass == AIClass.Medic)
            {
                FindInjuredAlly();
            }

            nextTargetSearchTime =
                Time.time +
                targetSearchInterval;
        }

        // =====================================
        // SQUAD ORDERS HAVE HIGHEST PRIORITY
        // =====================================

        if (squadOrder == SquadOrder.Follow)
        {
            HandleSquadFollow();
            return;
        }

        if (squadOrder == SquadOrder.Hold)
        {
            HandleSquadHold();
            return;
        }

        // =====================================
        // MEDIC
        // =====================================

        if (aiClass == AIClass.Medic)
        {
            if (injuredAlly != null)
            {
                HandleMedic();
                return;
            }

            if (currentEnemy != null)
            {
                HandleEnemy();
                return;
            }

            if (currentObjective != null)
            {
                HandleObjective();
                return;
            }

            StopMoving();

            return;
        }

        // =====================================
        // NORMAL SOLDIER
        // =====================================

        if (currentEnemy != null)
        {
            HandleEnemy();
            return;
        }

        if (currentObjective != null)
        {
            HandleObjective();
            return;
        }

        StopMoving();
    }

    // =========================================
    // SQUAD COMMANDS
    // =========================================

    public void SetSquadFollow()
    {
        squadOrder =
            SquadOrder.Follow;
    }

    public void SetSquadHold()
    {
        squadOrder =
            SquadOrder.Hold;

        holdPosition =
            transform.position;
    }

    public void SetFormationPosition(
        Vector3 position
    )
    {
        formationPosition =
            position;
    }

    public void ClearSquadOrder()
    {
        squadOrder =
            SquadOrder.None;
    }

    private void HandleSquadFollow()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                formationPosition
            );

        // Fight enemies that are close,
        // but don't permanently abandon
        // the player.
        if (currentEnemy != null)
        {
            float enemyDistance =
                Vector3.Distance(
                    transform.position,
                    currentEnemy.position
                );

            if (enemyDistance <= shootRange)
            {
                StopAndShoot();
                return;
            }
        }

        if (distance > squadStopDistance)
        {
            MoveTo(
                formationPosition
            );
        }
        else
        {
            StopMoving();
        }
    }

    private void HandleSquadHold()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                holdPosition
            );

        // Return to assigned hold position
        // if we somehow moved away.
        if (distance > squadStopDistance)
        {
            MoveTo(
                holdPosition
            );

            return;
        }

        // We reached the hold point.
        StopMoving();

        // Shoot enemies from here,
        // but DO NOT chase them.
        if (currentEnemy != null)
        {
            float enemyDistance =
                Vector3.Distance(
                    transform.position,
                    currentEnemy.position
                );

            if (enemyDistance <= shootRange)
            {
                FaceTarget(
                    currentEnemy.position
                );

                if (Time.time >= nextFireTime)
                {
                    Shoot();

                    nextFireTime =
                        Time.time +
                        fireRate;
                }
            }
        }
    }

    // =========================================
    // MEDIC
    // =========================================

    private void FindInjuredAlly()
    {
        TeamMember[] allTeamMembers =
            FindObjectsByType<TeamMember>(
                FindObjectsSortMode.None
            );

        float closestDistance =
            medicSearchRange;

        EnemyHealth closestInjuredAlly =
            null;

        foreach (TeamMember member in allTeamMembers)
        {
            if (member == null)
                continue;

            if (member == myTeam)
                continue;

            if (member.team != myTeam.team)
                continue;

            EnemyHealth health =
                member.GetComponent<EnemyHealth>();

            if (health == null)
                continue;

            if (!health.IsInjured)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    member.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance =
                    distance;

                closestInjuredAlly =
                    health;
            }
        }

        injuredAlly =
            closestInjuredAlly;
    }

    private void HandleMedic()
    {
        if (injuredAlly == null)
            return;

        if (!injuredAlly.IsInjured)
        {
            injuredAlly = null;
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                injuredAlly.transform.position
            );

        if (distance > healRange)
        {
            MoveTo(
                injuredAlly.transform.position
            );

            return;
        }

        StopMoving();

        FaceTarget(
            injuredAlly.transform.position
        );

        if (Time.time >= nextHealTime)
        {
            injuredAlly.Heal(
                healAmount
            );

            nextHealTime =
                Time.time +
                healInterval;
        }
    }

    // =========================================
    // TARGETING
    // =========================================

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
                closestDistance =
                    distance;

                closestTarget =
                    member.transform;
            }
        }

        currentEnemy =
            closestTarget;
    }

    // =========================================
    // COMMAND POSTS
    // =========================================

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

    // =========================================
    // NORMAL COMBAT
    // =========================================

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

        if (distance > objectiveStopDistance)
        {
            MoveTo(
                currentObjective.transform.position
            );
        }
        else
        {
            StopMoving();
        }
    }

    // =========================================
    // MOVEMENT
    // =========================================

    private void MoveTo(Vector3 position)
    {
        if (agent == null)
            return;

        agent.isStopped = false;

        agent.SetDestination(
            position
        );
    }

    private void StopMoving()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    // =========================================
    // SHOOTING
    // =========================================

    private void StopAndShoot()
    {
        if (currentEnemy == null)
            return;

        StopMoving();

        FaceTarget(
            currentEnemy.position
        );

        if (Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime =
                Time.time +
                fireRate;
        }
    }

    private void FaceTarget(
        Vector3 targetPosition
    )
    {
        Vector3 direction =
            targetPosition -
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