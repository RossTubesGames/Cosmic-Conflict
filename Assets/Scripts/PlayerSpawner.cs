using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Selected Character")]
    public GameObject playerPrefab;

    [Header("Player Team")]
    public Team playerTeam = Team.Human;

    [Header("Test Command Post")]
    public CommandPost testCommandPost;

    private GameObject currentPlayer;

    public void SetPlayerPrefab(GameObject newPlayerPrefab)
    {
        playerPrefab = newPlayerPrefab;

        if (newPlayerPrefab != null)
        {
            Debug.Log(
                "Selected character: " +
                newPlayerPrefab.name
            );
        }
    }

    public void SetTeam(Team newTeam)
    {
        playerTeam = newTeam;
    }

    public void SpawnPlayerAtCommandPost(
        CommandPost commandPost
    )
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning(
                "PlayerSpawner: No character selected."
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

        if (commandPost.ownerTeam != playerTeam)
        {
            Debug.LogWarning(
                "Cannot spawn here. Command Post belongs to "
                + commandPost.ownerTeam
            );

            return;
        }

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

        if (Camera.main != null)
        {
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
        }

        Debug.Log(
            playerPrefab.name +
            " spawned at " +
            commandPost.name
        );
    }
}