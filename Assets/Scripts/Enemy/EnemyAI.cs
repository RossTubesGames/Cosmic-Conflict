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

    [Header("Grenades")]
    public GameObject grenadePrefab;
    public Transform grenadeThrowPoint;

    public int maxGrenades = 4;

    public float grenadeMinRange = 5f;
    public float grenadeMaxRange = 15f;

    public float grenadeThrowForce = 10f;
    public float grenadeUpwardForce = 4f;

    public float grenadeThrowDelay = 2f;

    [Header("Grenade AI")]
    public float grenadeDecisionInterval = 3f;

    [Range(0f, 1f)]
    public float grenadeThrowChance = 0.25f;

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

    private EnemyHealth injuredAI;
    private PlayerHealth injuredPlayer;
    private Transform injuredTarget;

    private Vector3 formationPosition;
    private Vector3 holdPosition;

    private int currentGrenades;

    private float nextFireTime;
    private float nextHealTime;
    private float nextTargetSearchTime;

    private float nextGrenadeTime;
    private float nextGrenadeDecisionTime;

    private void Start()
    {
        if (agent == null)
        {
            agent =
                GetComponent<NavMeshAgent>();
        }

        myTeam =
            GetComponent<TeamMember>();

        currentGrenades =
            maxGrenades;

        FindEnemy();
        FindObjective();

        if (aiClass == AIClass.Medic)
        {
            FindInjuredAlly();
        }

        // Give each AI a slightly different
        // first grenade decision time.
        nextGrenadeDecisionTime =
            Time.time +
            Random.Range(
                0.5f,
                grenadeDecisionInterval
            );
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
        // GRENADE DECISION
        // =====================================

        if (aiClass != AIClass.Medic &&
            currentEnemy != null)
        {
            TryThrowGrenade();
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
            if (injuredTarget != null)
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
    // GRENADES
    // =========================================

    private void TryThrowGrenade()
    {
        if (grenadePrefab == null ||
            grenadeThrowPoint == null)
        {
            return;
        }

        if (currentEnemy == null)
            return;

        if (currentGrenades <= 0)
            return;

        if (Time.time < nextGrenadeTime)
            return;

        if (Time.time <
            nextGrenadeDecisionTime)
        {
            return;
        }

        // Only make a grenade decision
        // once every few seconds.
        nextGrenadeDecisionTime =
            Time.time +
            grenadeDecisionInterval;

        float distance =
            Vector3.Distance(
                transform.position,
                currentEnemy.position
            );

        // Don't throw if the enemy
        // is dangerously close.
        if (distance < grenadeMinRange)
            return;

        // Don't throw if the enemy
        // is too far away.
        if (distance > grenadeMaxRange)
            return;

        // Random chance prevents every AI
        // from throwing grenades together.
        if (Random.value >
            grenadeThrowChance)
        {
            return;
        }

        ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        if (currentEnemy == null)
            return;

        if (grenadePrefab == null ||
            grenadeThrowPoint == null)
        {
            return;
        }

        // Face the enemy before throwing.
        FaceTarget(
            currentEnemy.position
        );

        GameObject grenadeObject =
            Instantiate(
                grenadePrefab,
                grenadeThrowPoint.position,
                Quaternion.identity
            );

        Grenade grenade =
            grenadeObject.GetComponent<Grenade>();

        if (grenade != null)
        {
            grenade.team =
                myTeam.team;

            grenade.owner =
                transform.root;
        }

        Rigidbody rb =
            grenadeObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction =
                currentEnemy.position -
                grenadeThrowPoint.position;

            // We control the vertical throw
            // separately.
            direction.y = 0f;

            direction.Normalize();

            Vector3 throwVelocity =
                direction *
                grenadeThrowForce;

            throwVelocity +=
                Vector3.up *
                grenadeUpwardForce;

            rb.AddForce(
                throwVelocity,
                ForceMode.Impulse
            );
        }

        currentGrenades--;

        nextGrenadeTime =
            Time.time +
            grenadeThrowDelay;

        Debug.Log(
            gameObject.name +
            " threw grenade. Grenades left: " +
            currentGrenades
        );
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

        if (distance > squadStopDistance)
        {
            MoveTo(
                holdPosition
            );

            return;
        }

        StopMoving();

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

        EnemyHealth closestAI =
            null;

        PlayerHealth closestPlayer =
            null;

        Transform closestTarget =
            null;

        foreach (TeamMember member in allTeamMembers)
        {
            if (member == null)
                continue;

            if (member == myTeam)
                continue;

            // Medic only heals its own team.
            if (member.team != myTeam.team)
                continue;

            EnemyHealth enemyHealth =
                member.GetComponent<EnemyHealth>();

            PlayerHealth playerHealth =
                member.GetComponent<PlayerHealth>();

            bool needsHealing = false;

            if (enemyHealth != null &&
                enemyHealth.IsInjured)
            {
                needsHealing = true;
            }

            if (playerHealth != null &&
                playerHealth.IsInjured)
            {
                needsHealing = true;
            }

            if (!needsHealing)
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

                closestAI =
                    enemyHealth;

                closestPlayer =
                    playerHealth;
            }
        }

        injuredTarget =
            closestTarget;

        injuredAI =
            closestAI;

        injuredPlayer =
            closestPlayer;
    }

    private void HandleMedic()
    {
        if (injuredTarget == null)
            return;

        bool stillInjured = false;

        if (injuredAI != null &&
            injuredAI.IsInjured)
        {
            stillInjured = true;
        }

        if (injuredPlayer != null &&
            injuredPlayer.IsInjured)
        {
            stillInjured = true;
        }

        if (!stillInjured)
        {
            injuredAI = null;
            injuredPlayer = null;
            injuredTarget = null;

            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                injuredTarget.position
            );

        if (distance > healRange)
        {
            MoveTo(
                injuredTarget.position
            );

            return;
        }

        StopMoving();

        FaceTarget(
            injuredTarget.position
        );

        if (Time.time >= nextHealTime)
        {
            if (injuredAI != null)
            {
                injuredAI.Heal(
                    healAmount
                );
            }

            if (injuredPlayer != null)
            {
                injuredPlayer.Heal(
                    healAmount
                );
            }

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