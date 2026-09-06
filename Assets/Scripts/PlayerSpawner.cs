using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Prefab")]
    public GameObject playerPrefab;

    [Header("Player Team")]
    public Team playerTeam = Team.Human;

    [Header("Test Command Post")]
    public CommandPost testCommandPost;

    private GameObject currentPlayer;

    private void Update()
    {
        // TEMPORARY TEST
        // Later the UI button will call this instead.
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayerAtCommandPost(
                testCommandPost
            );
        }
    }

    public void SpawnPlayerAtCommandPost(
        CommandPost commandPost
    )
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning(
                "PlayerSpawner: No Player Prefab assigned."
            );

            return;
        }

        if (commandPost == null)
        {
            Debug.LogWarning(
                "PlayerSpawner: No Command Post selected."
            );

            return;
        }

        if (commandPost.playerSpawnPoint == null)
        {
            Debug.LogWarning(
                "PlayerSpawner: Command Post has no Player Spawn Point."
            );

            return;
        }

        // Only allow spawning at command posts
        // owned by the player's team.
        if (commandPost.ownerTeam != playerTeam)
        {
            Debug.LogWarning(
                "Cannot spawn here. Command Post belongs to "
                + commandPost.ownerTeam
            );

            return;
        }

        // Prevent accidentally creating multiple Players.
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(
            playerPrefab,
            commandPost.playerSpawnPoint.position,
            commandPost.playerSpawnPoint.rotation
        );

        TeamMember teamMember =
            currentPlayer.GetComponent<TeamMember>();

        if (teamMember != null)
        {
            teamMember.team =
                playerTeam;
        }

        // Immediately connect camera.
        ThirdPersonCamera cameraController =
            Camera.main.GetComponent<ThirdPersonCamera>();

        if (cameraController != null)
        {
            Transform camTarget =
                currentPlayer.transform.Find(
                    "CamTarget"
                );

            if (camTarget != null)
            {
                cameraController.SetTarget(
                    camTarget
                );
            }
        }

        Debug.Log(
            "Player spawned at "
            + commandPost.name
        );
    }
}