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

    // Choose what the crosshair is allowed
    // to detect in the Inspector.
    public LayerMask aimLayers = ~0;

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

        // =====================================
        // 1. RAY FROM CENTER OF CAMERA
        // =====================================

        Ray aimRay = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        Vector3 aimPoint;

        RaycastHit hit;

        // =====================================
        // 2. CHECK WHAT CROSSHAIR HITS
        // =====================================

        if (Physics.Raycast(
            aimRay,
            out hit,
            aimDistance,
            aimLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            // Crosshair hit something.
            // Shoot toward that exact point.
            aimPoint =
                hit.point;
        }
        else
        {
            // Crosshair hit nothing.
            // Aim far into the distance.
            aimPoint =
                Camera.main.transform.position +
                Camera.main.transform.forward *
                aimDistance;
        }

        // =====================================
        // 3. GUN -> CROSSHAIR TARGET
        // =====================================

        Vector3 shootDirection =
            (aimPoint - shootPoint.position)
            .normalized;

        Quaternion bulletRotation =
            Quaternion.LookRotation(
                shootDirection
            );

        // =====================================
        // 4. CREATE BULLET
        // =====================================

        GameObject bullet =
            Instantiate(
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