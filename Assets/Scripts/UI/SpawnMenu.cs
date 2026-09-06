using UnityEngine;
using UnityEngine.UI;

public class SpawnMenu : MonoBehaviour
{
    [Header("Spawner")]
    public PlayerSpawner playerSpawner;

    [Header("Command Posts")]
    public CommandPost westPost;
    public CommandPost eastPost;

    [Header("UI")]
    public GameObject spawnPanel;

    public Button westPostButton;
    public Button eastPostButton;

    private Team selectedTeam = Team.Human;

    private void Start()
    {
        SelectHuman();
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

        HideMenu();
    }

    public void SpawnAtEast()
    {
        playerSpawner.SpawnPlayerAtCommandPost(
            eastPost
        );

        HideMenu();
    }

    public void ShowMenu()
    {
        spawnPanel.SetActive(true);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        UpdatePostButtons();
    }

    public void HideMenu()
    {
        spawnPanel.SetActive(false);

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}