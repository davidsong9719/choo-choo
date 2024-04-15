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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playCard(card cardInfo, int cardPlayer)
    {
        switch (cardInfo.cardName)
        {
            case "increaseAttack":
                if (cardPlayer == 0)
                {
                    manager.bonusPlayerAttack += cardInfo.cardStrength;
                } else
                {
                    manager.bonusOpponentAttack += cardInfo.cardStrength;
                }
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
                break;

            case "increaseHand":
                if (cardPlayer == 0)
                {
                    if (manager.playerHandAmount < 5)
                    {
                        manager.playerHandAmount++;
                    }
                }
                break;
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
                break;
        }
    }

}
