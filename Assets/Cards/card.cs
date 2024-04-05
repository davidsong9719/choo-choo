using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/card")]
public class card : ScriptableObject
{
    public Sprite image;                        
                       
    public cardType type;
    public int cardStrength;

    //categorize item by 3 types
    public enum cardType
    {
        Attack,
        Defend,
        Effect
    }
}