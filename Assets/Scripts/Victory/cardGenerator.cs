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

    [TextArea]
    public string Notes = "";


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
                        if (randomFloat < 0.33f) //pray
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[0]);
                            curveValue = attackValueCurve.Evaluate(difficulty);
                            newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minAttack, maxAttack, curveValue)*1.5f);
                        }
                        else if (randomFloat < 0.66) //confuse
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[1]);
                        } else //increase hand
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[8]);
                        }
                        break;

                    case "gallium":
                        if (randomFloat < 0.33f) //curse
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[2]);
                        }
                        else if (randomFloat < 0.66) //increase defend
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[3]);
                            newCard.cardStrength = Random.Range(1, 3);
                        }
                        else //chainRetort
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[4]);
                            curveValue = defendValueCurve.Evaluate(difficulty);
                            newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minDefend, maxDefend, curveValue)*0.75f);
                        }
                        break;

                    case "pulse":
                        if (randomFloat < 0.33f) //increase attack
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[5]);
                            newCard.cardStrength = Random.Range(1, 5);
                        }
                        else if (randomFloat < 0.66) //life steal
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[6]);
                            curveValue = defendValueCurve.Evaluate(difficulty);
                            newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minDefend, maxDefend, curveValue));
                        }
                        else //outburst
                        {
                            newCard = copyCard(gameManager.instance.effectCardTemplates[7]);
                            curveValue = attackValueCurve.Evaluate(difficulty);
                            newCard.cardStrength = Mathf.RoundToInt(Mathf.Lerp(minAttack, maxAttack, curveValue) * 0.33f);
                        }
                        break;
                }
                break;
        }

        return newCard;
    }

    public card copyCard(card originalCard)
    {
        card newCard = ScriptableObject.CreateInstance<card>();
        newCard.type = originalCard.type;
        newCard.cardStrength = originalCard.cardStrength;
        newCard.cardName = originalCard.cardName;
        newCard.description = originalCard.description;
        return newCard;
    }
}
