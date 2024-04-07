using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardGenerator : MonoBehaviour
{
    public static cardGenerator instance; //set in gameManager

    [Header("Settings")]
    [SerializeField] AnimationCurve cardTypeDistribution; //0 is attack, 1 is defend, 2 is effect;
    [SerializeField] AnimationCurve defendValueCurve, attackValueCurve;
    [SerializeField] int maxVariation;
    [SerializeField] int minDefend, maxDefend, minAttack, maxAttack;

    public card generateNewCard(float difficulty) 
    {
        card newCard = ScriptableObject.CreateInstance<card>();
        int randomNum = Mathf.RoundToInt(cardTypeDistribution.Evaluate(Random.Range(0f, 1f)));
       float curveValue = 0;

        switch (randomNum)
        {
            case 0:
                newCard.type = card.cardType.Attack;
                curveValue = attackValueCurve.Evaluate(difficulty);
                newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minAttack, maxAttack, curveValue));
                newCard.cardStrength = Random.Range(newCard.cardStrength - maxVariation, newCard.cardStrength + maxVariation + 1);
                if (newCard.cardStrength < minAttack)
                {
                    newCard.cardStrength = minAttack;
                } else if (newCard.cardStrength > maxAttack)
                {
                    newCard.cardStrength = maxAttack;
                }
                break;

            case 1:
                newCard.type = card.cardType.Defend;
                curveValue = defendValueCurve.Evaluate(difficulty);
                newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minDefend, maxDefend, curveValue));
                newCard.cardStrength = Random.Range(newCard.cardStrength - maxVariation, newCard.cardStrength + maxVariation + 1);
                if (newCard.cardStrength < minDefend)
                {
                    newCard.cardStrength = minDefend;
                }
                else if (newCard.cardStrength > maxDefend)
                {
                    newCard.cardStrength = maxDefend;
                }
                break;

            case 2:
                newCard.type = card.cardType.Effect;
                break;
        }


        print("New " + newCard.type.ToString() + " card with the strength of " + newCard.cardStrength + " generated");

        return newCard;
    }
}
