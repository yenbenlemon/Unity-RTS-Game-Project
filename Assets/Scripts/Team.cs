using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    // ENUM used to dictate units team
    public enum TeamType
    {
        Player,
        Enemy
    };

    // ENUM used to dictate units type
    public enum UnitType
    {
        Static,
        Dynamic
    };

    // Allows us to know the team and unit type of a specific unit
    [Header("Team Selection")]
    public TeamType unitTeam = TeamType.Player;
    public UnitType unitType = UnitType.Static;
}
