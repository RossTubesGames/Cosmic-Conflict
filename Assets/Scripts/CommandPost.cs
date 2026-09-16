using System.Collections.Generic;
using UnityEngine;

public class CommandPost : MonoBehaviour
{
    public enum CommandPostOwner
    {
        Neutral,
        Human,
        Alien
    }

    [Header("Frontline")]
    public int frontlineOrder;

    [Header("Ownership")]
    public CommandPostOwner owner = CommandPostOwner.Neutral;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform aiSpawnPoint;

    [Header("Capture Settings")]
    public float captureRadius = 9f;
    public float captureTime = 10f;
    public float maxCaptureSpeedMultiplier = 5f;

    [Header("Detection")]
    public LayerMask captureLayers = ~0;

    [Header("Debug")]
    public int humansInside;
    public int aliensInside;

    [Range(-1f, 1f)]
    public float captureProgress = 0f;

    private void Update()
    {
        CountUnitsInside();
        HandleCapture();
    }

    private void CountUnitsInside()
    {
        humansInside = 0;
        aliensInside = 0;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            captureRadius,
            captureLayers,
            QueryTriggerInteraction.Ignore
        );

        // Prevent one character with multiple colliders
        // from being counted multiple times.
        HashSet<TeamMember> detectedUnits =
            new HashSet<TeamMember>();

        foreach (Collider col in colliders)
        {
            TeamMember teamMember =
                col.GetComponentInParent<TeamMember>();

            if (teamMember == null)
                continue;

            detectedUnits.Add(teamMember);
        }

        foreach (TeamMember unit in detectedUnits)
        {
            if (unit.team == Team.Human)
            {
                humansInside++;
            }
            else if (unit.team == Team.Alien)
            {
                aliensInside++;
            }
        }
    }

    private void HandleCapture()
    {
        // Nobody is capturing.
        if (humansInside == 0 &&
            aliensInside == 0)
        {
            return;
        }

        // Both factions are present.
        // The command post is contested.
        if (humansInside > 0 &&
            aliensInside > 0)
        {
            return;
        }

        if (humansInside > 0)
        {
            CaptureHuman();
        }
        else if (aliensInside > 0)
        {
            CaptureAlien();
        }
    }

    private void CaptureHuman()
    {
        float multiplier = Mathf.Clamp(
            humansInside,
            1,
            maxCaptureSpeedMultiplier
        );

        captureProgress +=
            (Time.deltaTime / captureTime) *
            multiplier;

        captureProgress =
            Mathf.Clamp(captureProgress, -1f, 1f);

        if (captureProgress >= 1f &&
            owner != CommandPostOwner.Human)
        {
            owner = CommandPostOwner.Human;

            Debug.Log(
                gameObject.name +
                " CAPTURED BY HUMANS!"
            );
        }
    }

    private void CaptureAlien()
    {
        float multiplier = Mathf.Clamp(
            aliensInside,
            1,
            maxCaptureSpeedMultiplier
        );

        captureProgress -=
            (Time.deltaTime / captureTime) *
            multiplier;

        captureProgress =
            Mathf.Clamp(captureProgress, -1f, 1f);

        if (captureProgress <= -1f &&
            owner != CommandPostOwner.Alien)
        {
            owner = CommandPostOwner.Alien;

            Debug.Log(
                gameObject.name +
                " CAPTURED BY ALIENS!"
            );
        }
    }

    public bool IsOwnedBy(Team team)
    {
        if (team == Team.Human)
        {
            return owner ==
                CommandPostOwner.Human;
        }

        if (team == Team.Alien)
        {
            return owner ==
                CommandPostOwner.Alien;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            captureRadius
        );
    }
}