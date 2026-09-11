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
        public List<GameObject> aliveUnits = new List<GameObject>();

        [HideInInspector]
        public bool isRespawning;
    }

    [Header("Team")]
    public Team team;

    [Header("Unit Types")]
    public UnitSpawnType[] unitTypes;

    [Header("Spawn Points")]
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

            if (unitType.aliveUnits.Count < unitType.maxAlive &&
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
            for (int i = 0; i < unitType.maxAlive; i++)
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

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
            return;

        Transform spawnPoint =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

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

    private void CleanupDeadUnits(
        UnitSpawnType unitType
    )
    {
        unitType.aliveUnits.RemoveAll(
            unit => unit == null
        );
    }
}