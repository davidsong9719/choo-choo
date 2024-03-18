using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class combatUI : MonoBehaviour
{
    [HideInInspector] public bool hasSelectedCard = false;

    [Header("Setup")]
    [SerializeField] RectTransform playerCircle; // speed display highlight
    [SerializeField] RectTransform opponentCircle; // ^

    [SerializeField] List<GameObject> playerCardDisplays;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<RectTransform> cardPositions;
    [SerializeField] Transform cardParent;
    [SerializeField] RectTransform playCardBorder;
    [SerializeField] RectTransform drawDeckPosition, discardDeckPosition;


    [Header("Settings")]
    [SerializeField] float speedCircleMoveAmount; // distance between the numbers on the speed displays

    private Vector3 playerCircleDefaultPos, opponentCircleDefaultPos;

    public void updateSpeedUI(int opponentSpeedCounter, int playerSpeedCounter)
    {
        playerCircle.anchoredPosition = new Vector3(playerCircleDefaultPos.x + speedCircleMoveAmount * playerSpeedCounter, playerCircleDefaultPos.y, 0);
        opponentCircle.anchoredPosition = new Vector3(opponentCircleDefaultPos.x + speedCircleMoveAmount * opponentSpeedCounter, opponentCircleDefaultPos.y, 0);
    }

    public void setDefaultPositions()
    {
        playerCircleDefaultPos = playerCircle.anchoredPosition;
        opponentCircleDefaultPos = opponentCircle.anchoredPosition;
    }

    public void spawnCard(card cardInfo, bool hasSpawnPosition, Vector3 spawnPosition)
    {
        GameObject newCard = Instantiate(cardPrefab, cardParent);
        newCard.GetComponent<Image>().sprite = cardInfo.image;
        
        cardFeedback cardScript = newCard.GetComponent<cardFeedback>();
        cardScript.uiController = this;
        cardScript.cardInfo = cardInfo;
        cardScript.cardState = "hand";
        cardScript.playBorder = playCardBorder;
        
        if (!hasSpawnPosition)
        {
            cardScript.cardTransform.anchoredPosition = drawDeckPosition.anchoredPosition;
        } else
        {
            cardScript.cardTransform.anchoredPosition = spawnPosition;
        }

        playerCardDisplays.Add(newCard);

        orderCards();

    }

    private void orderCards()
    {
        //place cards in hand depending on whether or not amount of cards in hand is even

        //layout cards
        if (playerCardDisplays.Count % 2 == 0) //even
        {
            for (int i = 0; i < playerCardDisplays.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        setCardTransform(0, 1, i, 1);
                        break;

                    case 1:
                        setCardTransform(1, 2, i, 2);
                        break;

                    case 2:
                        setCardTransform(2, 3, i, 3);
                        break;

                    case 3:
                        setCardTransform(3, 4, i, 4);
                        break;

                }
            }
        } else //odd
        {
            for (int i = 0; i < playerCardDisplays.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        setCardTransform(0, -1, i, 1);
                        break;

                    case 1:
                        setCardTransform(1, -1, i, 2);
                        break;

                    case 2:
                        setCardTransform(2, -1, i, 3);
                        break;

                    case 3:
                        setCardTransform(3, -1, i, 4);
                        break;

                    case 4:
                        setCardTransform(4, -1, i, 5);
                        break;
                }
            }
        }
    }

    private void setCardTransform(int cardPositionIndex0, int cardPositionIndex1, int playerCardDisplayIndex, int sortingIndex)
    {
        RectTransform cardDisplayTransform = playerCardDisplays[playerCardDisplayIndex].GetComponent<RectTransform>();
        cardFeedback cardScript = playerCardDisplays[playerCardDisplayIndex].GetComponent<cardFeedback>();

        switch (playerCardDisplays.Count)
        {
            case 1:
                cardPositionIndex0 += 2;
                break;

            case 2:
                cardPositionIndex0 += 1;
                cardPositionIndex1 += 1;
                break;

            case 3:
                cardPositionIndex0 += 1;
                break;

        }

        RectTransform cardPosition0 = cardPositions[cardPositionIndex0];
        RectTransform cardPosition1;
        if (cardPositionIndex1 < 0)
        {
            cardPosition1 = null;
        } else
        {
            cardPosition1 = cardPositions[cardPositionIndex1];
        }
        


        if (cardPositionIndex1 < 0) //odd
        {
            cardScript.targetPosition = cardPosition0.anchoredPosition;
            cardScript.setRotation(cardPosition0.rotation);
        } else //even
        {
            cardScript.targetPosition = (cardPosition0.anchoredPosition + cardPosition1.anchoredPosition)/2;
            cardScript.setRotation(Quaternion.Lerp(cardPosition0.rotation, cardPosition1.rotation, 0.5f));
        }

        cardDisplayTransform.SetSiblingIndex(sortingIndex * 2);
        cardScript.sortingIndex = sortingIndex * 2; 
    }

    public void selectCard(GameObject selectedCard)
    {
        playerCardDisplays.Remove(selectedCard);
        orderCards();
    }

    public void discardCard(GameObject discardedCard)
    {
        cardFeedback cardScript = discardedCard.GetComponent<cardFeedback>();
        cardScript.discardCard(discardDeckPosition.anchoredPosition);

        combatManager.instance.playerHand.Remove(cardScript.cardInfo);
        combatManager.instance.discardPile.Add(cardScript.cardInfo);
        playerCardDisplays.Remove(discardedCard);

    }
    
    public void discardAllCards()
    {
        while (playerCardDisplays.Count > 0)
        {
            discardCard(playerCardDisplays[0]);
        }
    }
} 
