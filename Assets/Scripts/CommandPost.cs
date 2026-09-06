using UnityEngine;

public class CommandPost : MonoBehaviour
{
    [Header("Ownership")]
    public Team ownerTeam;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform aiSpawnPoint;
}