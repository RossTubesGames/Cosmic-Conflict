using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject grenadePrefab;
    public Transform throwPoint;

    [Header("Grenade Ammo")]
    public int maxGrenades = 4;

    [Header("Throw")]
    public KeyCode grenadeKey = KeyCode.Q;
    public float throwForce = 12f;
    public float upwardForce = 4f;

    [Header("Throw Delay")]
    public float throwDelay = 2f;

    private int currentGrenades;
    private float nextThrowTime = 0f;

    private TeamMember teamMember;

    public int CurrentGrenades
    {
        get { return currentGrenades; }
    }

    private void Start()
    {
        currentGrenades =
            maxGrenades;

        teamMember =
            GetComponent<TeamMember>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grenadeKey))
        {
            TryThrowGrenade();
        }
    }

    private void TryThrowGrenade()
    {
        // Still waiting before another grenade
        // can be thrown.
        if (Time.time < nextThrowTime)
        {
            Debug.Log(
                "Grenade not ready yet."
            );

            return;
        }

        // No grenades remaining.
        if (currentGrenades <= 0)
        {
            Debug.Log(
                "NO GRENADES LEFT!"
            );

            return;
        }

        if (grenadePrefab == null ||
            throwPoint == null)
        {
            return;
        }

        ThrowGrenade();

        // Start the delay AFTER
        // successfully throwing.
        nextThrowTime =
            Time.time + throwDelay;
    }

    private void ThrowGrenade()
    {
        GameObject grenadeObject =
            Instantiate(
                grenadePrefab,
                throwPoint.position,
                Quaternion.identity
            );

        Grenade grenade =
            grenadeObject.GetComponent<Grenade>();

        if (grenade != null)
        {
            if (teamMember != null)
            {
                grenade.team =
                    teamMember.team;
            }

            grenade.owner =
                transform.root;
        }

        Rigidbody rb =
            grenadeObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection =
                transform.forward *
                throwForce;

            throwDirection +=
                Vector3.up *
                upwardForce;

            rb.AddForce(
                throwDirection,
                ForceMode.Impulse
            );
        }

        currentGrenades--;

        Debug.Log(
            "GRENADES: " +
            currentGrenades +
            " / " +
            maxGrenades
        );
    }
}