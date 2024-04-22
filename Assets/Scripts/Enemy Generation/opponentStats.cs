using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Opponent Stats")]

public class opponentStats : ScriptableObject
{
    public int health;
    public int speed;
    public int defense;
    public int attack;
    public float aggression;
    public float difficulty;
}
