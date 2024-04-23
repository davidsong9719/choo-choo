using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardEffect : MonoBehaviour
{
    private combatManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<combatManager>();
    }

    public void playCard(card cardInfo, int cardPlayer)
    {
        switch (cardInfo.cardName)
        {
            //Player
            case "redraw":
                if (cardPlayer == 0)
                {
                    manager.isPlayerToRedraw = true;
                }
                else if (cardPlayer == 1)
                {
                    manager.isOpponentToRedraw = true;
                }
                subwayUI.instance.setGuideTextPerm("Redrew hand");
                break;

            //tier 1
            case "increaseAttack":
                if (cardPlayer == 0)
                {
                    manager.bonusPlayerAttack += cardInfo.cardStrength;
                } else
                {
                    manager.bonusOpponentAttack += cardInfo.cardStrength;
                }
                subwayUI.instance.setGuideTextPerm("Future Argues improved by " + cardInfo.cardStrength);
                break;

            case "increaseDefend":
                if (cardPlayer == 0)
                {
                    manager.bonusPlayerDefend += cardInfo.cardStrength;
                }
                else
                {
                    manager.bonusOpponentDefend += cardInfo.cardStrength;
                }
                subwayUI.instance.setGuideTextPerm("Future Retort improved by " + cardInfo.cardStrength);
                break;

            case "increaseHand":
                if (cardPlayer == 0)
                {
                    if (manager.playerHandAmount < 5)
                    {
                        manager.playerHandAmount++;
                    }
                }
                subwayUI.instance.setGuideTextPerm("Hand size increased to " + manager.playerHandAmount);
                break;

            //tier 2
            case "lifeSteal":
                if (cardPlayer == 0)
                {
                    manager.inflictSimpleDamage(1, cardInfo.cardStrength);
                    manager.heal(0, cardInfo.cardStrength);
                } else
                {
                    manager.inflictSimpleDamage(0, cardInfo.cardStrength);
                    manager.heal(1, cardInfo.cardStrength);
                }
                subwayUI.instance.setGuideTextPerm("Drained " + cardInfo.cardStrength + " Willpower");
                break;

            case "curse":
                if (cardPlayer == 0)
                {
                    manager.isOpponentCursed = true;
                }
                else if (cardPlayer == 1)
                {
                    manager.isPlayerCursed = true;
                }
                subwayUI.instance.setGuideTextPerm("Cursed!");
                break;
                
            case "pray":
                float randomNum = Random.Range(0f, 1f);
                bool isSuccessful = randomNum > 0.66;
                if (cardPlayer == 0)
                {
                    if (isSuccessful)
                    {
                        manager.inflictSimpleDamage(1, cardInfo.cardStrength);
                    }
                    else
                    {
                        manager.isPlayerCursed = true;
                        manager.endPlayerTurnCursed = true;
                    }
                }
                else if (cardPlayer == 1)
                {
                    if (isSuccessful)
                    {
                        manager.inflictSimpleDamage(0, cardInfo.cardStrength);
                    }
                    else
                    {
                        manager.isOpponentCursed = true;
                        manager.endOpponentTurnCursed = true;
                    }
                }

                if (isSuccessful)
                {
                    subwayUI.instance.setGuideTextPerm("Roll succeeded, decreased Willpower by " + cardInfo.cardStrength);
                } else
                {
                    subwayUI.instance.setGuideTextPerm("Roll failed, became Cursed!");
                }
                break;


            //tier 3
            case "outburst":
                if (cardPlayer == 0)
                {
                    manager.inflictDamage(1, cardInfo.cardStrength*3);
                } else if (cardPlayer == 1)
                {
                    manager.inflictSimpleDamage(0, cardInfo.cardStrength*3);
                }
                break;

            case "chainRetort":
                if (cardPlayer == 0)
                {
                    manager.simpleRetort(0, cardInfo.cardStrength);
                    manager.isPlayerMultiPlay = true;

                } else if (cardPlayer == 1)
                {
                    manager.isOpponentMultiPlay = true;
                    manager.simpleRetort(1, cardInfo.cardStrength);
                }
                break;

            case "confuse":
                if (cardPlayer == 0)
                {
                    manager.decreaseSpeed(1);
                } else if (cardPlayer == 1)
                {
                    manager.decreaseSpeed(0);
                }

                break;
        }
    }

}
