using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Transform shootPoint;
    public GameObject bulletPrefab;

    [Header("Weapon Stats")]
    public float fireRate = 0.3f;
    public float bulletSpeed = 40f;
    public float bulletDamage = 25f;
    public float bulletLifetime = 10f;

    private float nextFireTime;

    private TeamMember teamMember;

    private void Start()
    {
        teamMember =
            GetComponentInParent<TeamMember>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) &&
            Time.time >= nextFireTime)
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
            if (teamMember != null)
            {
                projectile.team =
                    teamMember.team;
            }

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