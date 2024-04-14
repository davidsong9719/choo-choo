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

    public void playCard(card cardInfo, int target)
    {
        switch (cardInfo.type)
        {
            case card.cardType.Attack:
                manager.inflictDamage(target, cardInfo.cardStrength);
                break;

            case card.cardType.Defend:
                manager.retort(target, cardInfo.cardStrength);
                break;

            case card.cardType.Effect:
                break;
        }
    }

    private void defend(card cardInfo, int target)
    {

    }

    private void effect(card cardInfo, int target)
    {

    }

}
