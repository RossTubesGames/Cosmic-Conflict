using UnityEngine;

public enum Team
{
    Human,
    Alien
}

public class TeamMember : MonoBehaviour
{
    [Header("Team")]
    public Team team;
}