using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSpawner : MonoBehaviour
{
    [System.Serializable]
    public class UnitSpawnType
    {
        public string unitName;
        public GameObject unitPrefab;
        public int maxAlive = 3;

        [HideInInspector]
        public List<GameObject> aliveUnits =
            new List<GameObject>();

        [HideInInspector]
        public bool isRespawning;
    }

    [Header("Team")]
    public Team team;

    [Header("Unit Types")]
    public UnitSpawnType[] unitTypes;

    [Header("Fallback Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Respawn")]
    public float respawnDelay = 3f;

    private void Start()
    {
        SpawnStartingArmy();
    }

    private void Update()
    {
        foreach (UnitSpawnType unitType in unitTypes)
        {
            CleanupDeadUnits(unitType);

            if (unitType.aliveUnits.Count <
                unitType.maxAlive &&
                !unitType.isRespawning)
            {
                StartCoroutine(
                    RespawnUnit(unitType)
                );
            }
        }
    }

    private void SpawnStartingArmy()
    {
        foreach (UnitSpawnType unitType in unitTypes)
        {
            for (int i = 0;
                 i < unitType.maxAlive;
                 i++)
            {
                SpawnUnit(unitType);
            }
        }
    }

    private IEnumerator RespawnUnit(
        UnitSpawnType unitType
    )
    {
        unitType.isRespawning = true;

        yield return new WaitForSeconds(
            respawnDelay
        );

        SpawnUnit(unitType);

        unitType.isRespawning = false;
    }

    private void SpawnUnit(
        UnitSpawnType unitType
    )
    {
        if (unitType.unitPrefab == null)
            return;

        Transform spawnPoint =
            GetFrontlineSpawnPoint();

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                team +
                " has no valid AI spawn point!"
            );

            return;
        }

        GameObject newUnit =
            Instantiate(
                unitType.unitPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        TeamMember teamMember =
            newUnit.GetComponent<TeamMember>();

        if (teamMember != null)
        {
            teamMember.team = team;
        }

        unitType.aliveUnits.Add(
            newUnit
        );
    }

    private Transform GetFrontlineSpawnPoint()
    {
        CommandPost[] commandPosts =
            FindObjectsByType<CommandPost>(
                FindObjectsSortMode.None
            );

        CommandPost bestPost = null;

        foreach (CommandPost post in commandPosts)
        {
            if (post == null)
                continue;

            if (!post.IsOwnedBy(team))
                continue;

            if (post.aiSpawnPoint == null)
                continue;

            // First valid post found.
            if (bestPost == null)
            {
                bestPost = post;
                continue;
            }

            // Humans advance West -> East.
            // Higher number = further forward.
            if (team == Team.Human)
            {
                if (post.frontlineOrder >
                    bestPost.frontlineOrder)
                {
                    bestPost = post;
                }
            }

            // Aliens advance East -> West.
            // Lower number = further forward.
            else if (team == Team.Alien)
            {
                if (post.frontlineOrder <
                    bestPost.frontlineOrder)
                {
                    bestPost = post;
                }
            }
        }

        if (bestPost != null)
        {
            return bestPost.aiSpawnPoint;
        }

        // Safety fallback to the old
        // spawn-point system.
        if (spawnPoints != null &&
            spawnPoints.Length > 0)
        {
            return spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];
        }

        return null;
    }

    private void CleanupDeadUnits(
        UnitSpawnType unitType
    )
    {
        unitType.aliveUnits.RemoveAll(
            unit => unit == null
        );
    }
}