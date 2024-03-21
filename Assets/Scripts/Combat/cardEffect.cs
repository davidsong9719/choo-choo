using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playCard(card cardInfo, int target)
    {
        if (target == 0) //targeting player
        {
            
        } else if (target == 1) //targets first enemy
        {
            switch (cardInfo.type)
            {
                case card.cardType.Attack:
                    break;

                case card.cardType.Defend:
                    break;

                case card.cardType.Effect:
                    break;
            }
        }
    }



}
