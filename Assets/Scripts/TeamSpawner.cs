using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSpawner : MonoBehaviour
{
    [Header("Team")]
    public Team team;

    [Header("Unit")]
    public GameObject unitPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Battle Settings")]
    public int maxAliveUnits = 3;
    public float respawnDelay = 3f;

    private List<GameObject> aliveUnits = new List<GameObject>();

    private bool isRespawning;

    private void Start()
    {
        SpawnStartingUnits();
    }

    private void Update()
    {
        CleanupDeadUnits();

        if (aliveUnits.Count < maxAliveUnits && !isRespawning)
        {
            StartCoroutine(RespawnUnit());
        }
    }

    private void SpawnStartingUnits()
    {
        for (int i = 0; i < maxAliveUnits; i++)
        {
            SpawnUnit();
        }
    }

    private IEnumerator RespawnUnit()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        SpawnUnit();

        isRespawning = false;
    }

    private void SpawnUnit()
    {
        if (unitPrefab == null)
            return;

        if (spawnPoints.Length == 0)
            return;

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newUnit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        TeamMember teamMember =
            newUnit.GetComponent<TeamMember>();

        if (teamMember != null)
        {
            teamMember.team = team;
        }

        aliveUnits.Add(newUnit);
    }

    private void CleanupDeadUnits()
    {
        aliveUnits.RemoveAll(
            unit => unit == null
        );
    }
}