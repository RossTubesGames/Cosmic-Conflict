using UnityEngine;
using TMPro;

public class FriendlyHealthUI : MonoBehaviour
{
    [Header("Detection")]
    public float lookRange = 20f;
    public LayerMask lookLayers = ~0;

    private TeamMember myTeam;
    private TMP_Text friendlyHealthText;

    private void Start()
    {
        myTeam =
            GetComponent<TeamMember>();

        FindUI();

        HideUI();
    }

    private void Update()
    {
        if (friendlyHealthText == null)
        {
            FindUI();
        }

        CheckFriendlyTarget();
    }

    private void FindUI()
    {
        FriendlyHealthDisplay display =
            FindFirstObjectByType<FriendlyHealthDisplay>(
                FindObjectsInactive.Include
            );

        if (display != null)
        {
            friendlyHealthText =
                display.healthText;
        }
    }

    private void CheckFriendlyTarget()
    {
        if (myTeam == null ||
            friendlyHealthText == null ||
            Camera.main == null)
        {
            HideUI();
            return;
        }

        Ray ray =
            new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward
            );

        RaycastHit hit;

        if (!Physics.Raycast(
            ray,
            out hit,
            lookRange,
            lookLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            HideUI();
            return;
        }

        TeamMember targetTeam =
            hit.collider.GetComponentInParent<TeamMember>();

        if (targetTeam == null)
        {
            HideUI();
            return;
        }

        // Enemy.
        if (targetTeam.team != myTeam.team)
        {
            HideUI();
            return;
        }

        // Our own player.
        if (targetTeam.transform.root ==
            transform.root)
        {
            HideUI();
            return;
        }

        EnemyHealth health =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (health == null)
        {
            HideUI();
            return;
        }

        ShowUI(
            health.CurrentHealth,
            health.maxHealth
        );
    }

    private void ShowUI(
        float currentHealth,
        float maxHealth
    )
    {
        if (friendlyHealthText == null)
            return;

        friendlyHealthText.gameObject.SetActive(true);

        friendlyHealthText.text =
            "ALLY\n" +
            Mathf.CeilToInt(currentHealth) +
            " / " +
            Mathf.CeilToInt(maxHealth);
    }

    private void HideUI()
    {
        if (friendlyHealthText != null)
        {
            friendlyHealthText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        HideUI();
    }
}