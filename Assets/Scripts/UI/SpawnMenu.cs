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
    public CommandPost eastPost;

    [Header("Spawn UI")]
    public GameObject spawnPanel;

    public Button westPostButton;
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
            spawnPanel.SetActive(true);

        if (deathPanel != null)
            deathPanel.SetActive(false);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        UpdatePostButtons();
    }

    public void SelectHuman()
    {
        selectedTeam = Team.Human;

        playerSpawner.playerTeam = selectedTeam;

        UpdatePostButtons();
    }

    public void SelectAlien()
    {
        selectedTeam = Team.Alien;

        playerSpawner.playerTeam = selectedTeam;

        UpdatePostButtons();
    }

    private void UpdatePostButtons()
    {
        if (westPostButton != null)
        {
            westPostButton.interactable =
                westPost.ownerTeam == selectedTeam;
        }

        if (eastPostButton != null)
        {
            eastPostButton.interactable =
                eastPost.ownerTeam == selectedTeam;
        }
    }

    public void SpawnAtWest()
    {
        playerSpawner.SpawnPlayerAtCommandPost(
            westPost
        );

        HideSpawnMenu();
    }

    public void SpawnAtEast()
    {
        playerSpawner.SpawnPlayerAtCommandPost(
            eastPost
        );

        HideSpawnMenu();
    }

    public void StartDeathScreen(GameObject player)
    {
        deadPlayer = player;

        if (spawnPanel != null)
            spawnPanel.SetActive(false);

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        respawnCoroutine =
            StartCoroutine(RespawnCountdown());
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
                    "Respawn in "
                    + Mathf.CeilToInt(timeRemaining);
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
            StopCoroutine(respawnCoroutine);
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (deadPlayer != null)
        {
            Destroy(deadPlayer);
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
            StopCoroutine(respawnCoroutine);
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (deadPlayer != null)
        {
            Destroy(deadPlayer);
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