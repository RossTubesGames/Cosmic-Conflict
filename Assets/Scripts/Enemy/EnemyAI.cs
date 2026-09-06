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

    [Header("Targeting")]
    public float targetSearchInterval = 0.5f;

    private TeamMember myTeam;
    private Transform currentTarget;
    private float nextFireTime;
    private float nextTargetSearchTime;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        myTeam = GetComponent<TeamMember>();

        FindTarget();
    }

    private void Update()
    {
        if (myTeam == null)
            return;

        if (Time.time >= nextTargetSearchTime)
        {
            FindTarget();
            nextTargetSearchTime = Time.time + targetSearchInterval;
        }

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            currentTarget.position
        );

        if (distance > shootRange)
        {
            MoveToTarget();
        }
        else
        {
            StopAndShoot();
        }
    }

    private void FindTarget()
    {
        TeamMember[] allTeamMembers =
            FindObjectsByType<TeamMember>(
                FindObjectsSortMode.None
            );

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (TeamMember member in allTeamMembers)
        {
            if (member == myTeam)
                continue;

            if (member.team == myTeam.team)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                member.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = member.transform;
            }
        }

        currentTarget = closestTarget;
    }

    private void MoveToTarget()
    {
        if (agent == null)
            return;

        agent.isStopped = false;

        agent.SetDestination(
            currentTarget.position
        );
    }

    private void StopAndShoot()
    {
        if (agent != null)
            agent.isStopped = true;

        Vector3 direction =
            currentTarget.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
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
        GameObject bullet = Instantiate(
            bulletPrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Projectile projectile =
            bullet.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.team = myTeam.team;
            projectile.owner = transform.root;
            projectile.speed = bulletSpeed;
            projectile.damage = bulletDamage;
            projectile.lifetime = bulletLifetime;
        }
    }
}