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

    [Header("Aiming")]
    public float aimDistance = 200f;

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
        if (shootPoint == null ||
            bulletPrefab == null ||
            Camera.main == null)
        {
            return;
        }

        // Aim far forward from the CENTER of the camera.
        // We do NOT use a close wall hit point anymore.
        Vector3 aimPoint =
            Camera.main.transform.position +
            Camera.main.transform.forward *
            aimDistance;

        // Bullet starts at the gun,
        // but travels toward the camera's aim direction.
        Vector3 shootDirection =
            (aimPoint - shootPoint.position).normalized;

        Quaternion bulletRotation =
            Quaternion.LookRotation(
                shootDirection
            );

        GameObject bullet = Instantiate(
            bulletPrefab,
            shootPoint.position,
            bulletRotation
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