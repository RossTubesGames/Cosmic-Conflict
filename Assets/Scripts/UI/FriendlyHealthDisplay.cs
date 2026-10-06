using TMPro;
using UnityEngine;

public class FriendlyHealthDisplay : MonoBehaviour
{
    public TMP_Text healthText;

    private void Awake()
    {
        if (healthText == null)
        {
            healthText =
                GetComponent<TMP_Text>();
        }
    }
}