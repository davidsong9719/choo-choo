using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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
                float randomFloat = Random.Range(0f, 1f);
                switch (nodeManager.instance.currentLine)
                {
                    case "pilgrim":
                        if (randomFloat < 0.33f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[0];
                        }
                        else if (randomFloat < 0.66)
                        {
                            newCard = gameManager.instance.effectCardTemplates[1];
                        } else
                        {
                            newCard = gameManager.instance.effectCardTemplates[8];
                        }
                        break;

                    case "gallium":
                        if (randomFloat < 0.33f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[2];
                        }
                        else if (randomFloat < 0.66)
                        {
                            newCard = gameManager.instance.effectCardTemplates[3];
                        }
                        else
                        {
                            newCard = gameManager.instance.effectCardTemplates[4];
                        }
                        break;

                    case "pulse":
                        if (randomFloat < 0.33f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[5];
                        }
                        else if (randomFloat < 0.66)
                        {
                            newCard = gameManager.instance.effectCardTemplates[6];
                        }
                        else
                        {
                            newCard = gameManager.instance.effectCardTemplates[7];
                        }
                        break;
                }
                break;
        }

        return newCard;
    }
}
