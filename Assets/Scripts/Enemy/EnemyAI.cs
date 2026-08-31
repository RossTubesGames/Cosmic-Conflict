using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
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

    private float nextFireTime;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > shootRange)
        {
            MoveToPlayer();
        }
        else
        {
            StopAndShoot();
        }
    }

    private void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void StopAndShoot()
    {
        agent.isStopped = true;

        Vector3 direction =
            player.position -
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
            projectile.team =
                Projectile.ProjectileTeam.Enemy;

            projectile.owner = transform.root;

            projectile.speed = bulletSpeed;
            projectile.damage = bulletDamage;
            projectile.lifetime = bulletLifetime;
        }
    }
}