using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMenu : MonoBehaviour
{
    [Header("Spawner")]
    public PlayerSpawner playerSpawner;

    [Header("Command Posts")]
    public CommandPost westPost;
    public CommandPost centralPost;
    public CommandPost eastPost;

    [Header("Spawn UI")]
    public GameObject spawnPanel;

    public Button westPostButton;
    public Button centralPostButton;
    public Button eastPostButton;

    [Header("Death UI")]
    public GameObject deathPanel;
    public TMP_Text countdownText;

    [Header("Death Settings")]
    public float respawnDelay = 10f;

    private Team selectedTeam = Team.Human;

    private GameObject deadPlayer;

    private Coroutine respawnCoroutine;

    private void Start()
    {
        SelectHuman();

        // No player exists at the beginning,
        // so show the spawn menu immediately.
        if (spawnPanel != null)
        {
            spawnPanel.SetActive(true);
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        UpdatePostButtons();
    }

    public void SelectHuman()
    {
        selectedTeam = Team.Human;

        if (playerSpawner != null)
        {
            playerSpawner.playerTeam =
                selectedTeam;
        }

        UpdatePostButtons();
    }

    public void SelectAlien()
    {
        selectedTeam = Team.Alien;

        if (playerSpawner != null)
        {
            playerSpawner.playerTeam =
                selectedTeam;
        }

        UpdatePostButtons();
    }

    private void UpdatePostButtons()
    {
        if (westPostButton != null &&
            westPost != null)
        {
            westPostButton.interactable =
                westPost.IsOwnedBy(selectedTeam);
        }

        if (centralPostButton != null &&
            centralPost != null)
        {
            centralPostButton.interactable =
                centralPost.IsOwnedBy(selectedTeam);
        }

        if (eastPostButton != null &&
            eastPost != null)
        {
            eastPostButton.interactable =
                eastPost.IsOwnedBy(selectedTeam);
        }
    }

    public void SpawnAtWest()
    {
        if (westPost == null)
            return;

        playerSpawner.SpawnPlayerAtCommandPost(
            westPost
        );

        HideSpawnMenu();
    }

    public void SpawnAtCentral()
    {
        if (centralPost == null)
            return;

        playerSpawner.SpawnPlayerAtCommandPost(
            centralPost
        );

        HideSpawnMenu();
    }

    public void SpawnAtEast()
    {
        if (eastPost == null)
            return;

        playerSpawner.SpawnPlayerAtCommandPost(
            eastPost
        );

        HideSpawnMenu();
    }

    public void StartDeathScreen(GameObject player)
    {
        deadPlayer = player;

        if (spawnPanel != null)
        {
            spawnPanel.SetActive(false);
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        respawnCoroutine =
            StartCoroutine(
                RespawnCountdown()
            );
    }

    private IEnumerator RespawnCountdown()
    {
        float timeRemaining =
            respawnDelay;

        while (timeRemaining > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text =
                    "Respawn in " +
                    Mathf.CeilToInt(
                        timeRemaining
                    );
            }

            timeRemaining -=
                Time.deltaTime;

            yield return null;
        }

        OpenSpawnMenu();
    }

    public void RespawnButtonPressed()
    {
        OpenSpawnMenu();
    }

    public void ChangeCharacterButtonPressed()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(
                respawnCoroutine
            );

            respawnCoroutine = null;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (deadPlayer != null)
        {
            Destroy(deadPlayer);

            deadPlayer = null;
        }

        CharacterSelectMenu characterMenu =
            FindFirstObjectByType<CharacterSelectMenu>();

        if (characterMenu != null)
        {
            characterMenu.OpenCharacterMenu();
        }
    }

    private void OpenSpawnMenu()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(
                respawnCoroutine
            );

            respawnCoroutine = null;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (deadPlayer != null)
        {
            Destroy(deadPlayer);

            deadPlayer = null;
        }

        if (spawnPanel != null)
        {
            spawnPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        UpdatePostButtons();
    }

    private void HideSpawnMenu()
    {
        if (spawnPanel != null)
        {
            spawnPanel.SetActive(false);
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}