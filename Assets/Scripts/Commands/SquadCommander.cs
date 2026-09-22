using System.Collections.Generic;
using UnityEngine;

public class SquadCommander : MonoBehaviour
{
    [Header("Follow Command")]
    public int maxFollowers = 2;
    public float followCommandRange = 15f;

    [Header("Hold / Free Commands")]
    public int maxAreaCommandUnits = 4;
    public float areaCommandRange = 15f;

    [Header("Formation")]
    public float followDistance = 2.5f;
    public float sideDistance = 1.5f;

    [Header("Controls")]
    public KeyCode followKey = KeyCode.F;
    public KeyCode holdKey = KeyCode.G;
    public KeyCode freeKey = KeyCode.H;

    private TeamMember myTeam;

    private List<EnemyAI> followers =
        new List<EnemyAI>();

    private void Start()
    {
        myTeam = GetComponent<TeamMember>();
    }

    private void Update()
    {
        CleanupFollowers();

        if (Input.GetKeyDown(followKey))
        {
            FollowCommand();
        }

        if (Input.GetKeyDown(holdKey))
        {
            HoldCommand();
        }

        if (Input.GetKeyDown(freeKey))
        {
            FreeCommand();
        }

        UpdateFormationPositions();
    }

    // =========================================
    // FOLLOW
    // =========================================

    private void FollowCommand()
    {
        if (myTeam == null)
            return;

        // If we already have followers,
        // tell them to follow again.
        if (followers.Count > 0)
        {
            foreach (EnemyAI ally in followers)
            {
                if (ally != null)
                {
                    ally.SetSquadFollow();
                }
            }

            Debug.Log("COMMAND: FOLLOW ME!");

            return;
        }

        List<EnemyAI> nearbyAllies =
            FindClosestFriendlyAI(
                followCommandRange
            );

        int amount =
            Mathf.Min(
                maxFollowers,
                nearbyAllies.Count
            );

        for (int i = 0; i < amount; i++)
        {
            EnemyAI ally =
                nearbyAllies[i];

            followers.Add(ally);

            ally.SetSquadFollow();
        }

        Debug.Log(
            "COMMAND: " +
            followers.Count +
            " ALLIES FOLLOWING!"
        );
    }

    // =========================================
    // HOLD
    // =========================================

    private void HoldCommand()
    {
        if (myTeam == null)
            return;

        List<EnemyAI> nearbyAllies =
            FindClosestFriendlyAI(
                areaCommandRange
            );

        int amount =
            Mathf.Min(
                maxAreaCommandUnits,
                nearbyAllies.Count
            );

        for (int i = 0; i < amount; i++)
        {
            nearbyAllies[i].SetSquadHold();
        }

        Debug.Log(
            "COMMAND: " +
            amount +
            " ALLIES HOLDING POSITION!"
        );
    }

    // =========================================
    // FREE / RESUME
    // =========================================

    private void FreeCommand()
    {
        if (myTeam == null)
            return;

        List<EnemyAI> nearbyAllies =
            FindClosestFriendlyAI(
                areaCommandRange
            );

        int amount =
            Mathf.Min(
                maxAreaCommandUnits,
                nearbyAllies.Count
            );

        for (int i = 0; i < amount; i++)
        {
            EnemyAI ally =
                nearbyAllies[i];

            ally.ClearSquadOrder();

            followers.Remove(ally);
        }

        Debug.Log(
            "COMMAND: " +
            amount +
            " ALLIES RELEASED!"
        );
    }

    // =========================================
    // FIND FRIENDLY AI
    // =========================================

    private List<EnemyAI> FindClosestFriendlyAI(
        float range
    )
    {
        EnemyAI[] allAI =
            FindObjectsByType<EnemyAI>(
                FindObjectsSortMode.None
            );

        List<EnemyAI> friendlyAI =
            new List<EnemyAI>();

        foreach (EnemyAI ai in allAI)
        {
            if (ai == null)
                continue;

            TeamMember teamMember =
                ai.GetComponent<TeamMember>();

            if (teamMember == null)
                continue;

            if (teamMember.team != myTeam.team)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    ai.transform.position
                );

            if (distance <= range)
            {
                friendlyAI.Add(ai);
            }
        }

        friendlyAI.Sort(
            (a, b) =>
                Vector3.Distance(
                    transform.position,
                    a.transform.position
                ).CompareTo(
                    Vector3.Distance(
                        transform.position,
                        b.transform.position
                    )
                )
        );

        return friendlyAI;
    }

    // =========================================
    // FORMATION
    // =========================================

    private void UpdateFormationPositions()
    {
        if (followers.Count == 0)
            return;

        for (int i = 0;
             i < followers.Count;
             i++)
        {
            EnemyAI ally =
                followers[i];

            if (ally == null)
                continue;

            Vector3 sideOffset;

            if (i == 0)
            {
                sideOffset =
                    -transform.right *
                    sideDistance;
            }
            else
            {
                sideOffset =
                    transform.right *
                    sideDistance;
            }

            Vector3 formationPosition =
                transform.position
                - transform.forward *
                followDistance
                + sideOffset;

            ally.SetFormationPosition(
                formationPosition
            );
        }
    }

    // =========================================
    // PLAYER DEATH / DESTRUCTION
    // =========================================

    private void OnDestroy()
    {
        ReleaseAllFollowers();
    }

    private void ReleaseAllFollowers()
    {
        foreach (EnemyAI ally in followers)
        {
            if (ally != null)
            {
                ally.ClearSquadOrder();
            }
        }

        followers.Clear();
    }

    private void CleanupFollowers()
    {
        followers.RemoveAll(
            ally => ally == null
        );
    }
}