using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/card")]
public class card : ScriptableObject
{
    public cardType type;
    public int cardStrength;

    [Header("Effect Card Variables")]
    public string cardName;
    [TextArea(15, 20)] public string description;

    //categorize item by 3 types
    public enum cardType
    {
        Attack,
        Defend,
        Effect,
        Cursed
    }
}