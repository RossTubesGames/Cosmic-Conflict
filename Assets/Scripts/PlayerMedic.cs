using UnityEngine;

public class PlayerMedic : MonoBehaviour
{
    [Header("Healing")]
    public KeyCode healKey = KeyCode.E;
    public float healRange = 5f;
    public float healAmount = 20f;
    public float healInterval = 1f;

    [Header("Targeting")]
    public LayerMask healLayers = ~0;

    private TeamMember myTeam;
    private float nextHealTime;

    private void Start()
    {
        myTeam =
            GetComponent<TeamMember>();
    }

    private void Update()
    {
        if (Input.GetKey(healKey))
        {
            TryHeal();
        }
    }

    private void TryHeal()
    {
        if (myTeam == null)
            return;

        if (Camera.main == null)
            return;

        if (Time.time < nextHealTime)
            return;

        Ray ray =
            new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward
            );

        RaycastHit hit;

        if (!Physics.Raycast(
            ray,
            out hit,
            healRange,
            healLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            return;
        }

        TeamMember targetTeam =
            hit.collider.GetComponentInParent<TeamMember>();

        if (targetTeam == null)
            return;

        // Only heal our own faction.
        if (targetTeam.team != myTeam.team)
            return;

        // Don't heal ourselves.
        if (targetTeam.transform.root ==
            transform.root)
        {
            return;
        }

        EnemyHealth enemyHealth =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null)
            return;

        if (!enemyHealth.IsInjured)
            return;

        enemyHealth.Heal(
            healAmount
        );

        nextHealTime =
            Time.time +
            healInterval;

        Debug.Log(
            "Medic healed " +
            enemyHealth.gameObject.name +
            " for " +
            healAmount +
            " HP."
        );
    }
}
