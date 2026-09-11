using UnityEngine;

public class CharacterSelectMenu : MonoBehaviour
{
    [Header("References")]
    public PlayerSpawner playerSpawner;

    [Header("Player Prefabs")]
    public GameObject assaultPrefab;
    public GameObject heavyPrefab;

    [Header("UI Panels")]
    public GameObject characterPanel;
    public GameObject spawnPanel;

    public void SelectAssault()
    {
        playerSpawner.SetPlayerPrefab(
            assaultPrefab
        );

        OpenSpawnMenu();
    }

    public void SelectHeavy()
    {
        playerSpawner.SetPlayerPrefab(
            heavyPrefab
        );

        OpenSpawnMenu();
    }

    private void OpenSpawnMenu()
    {
        if (characterPanel != null)
        {
            characterPanel.SetActive(false);
        }

        if (spawnPanel != null)
        {
            spawnPanel.SetActive(true);
        }
    }

    public void OpenCharacterMenu()
    {
        if (spawnPanel != null)
        {
            spawnPanel.SetActive(false);
        }

        if (characterPanel != null)
        {
            characterPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }
}
