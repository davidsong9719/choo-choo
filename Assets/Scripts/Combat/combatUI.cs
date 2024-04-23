using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class combatUI : MonoBehaviour
{
    [HideInInspector] public bool hasSelectedCard = false;

    [Header("Setup")]
    [SerializeField] speedDisplays playerSpeedDisplay;
    [SerializeField] speedDisplays opponentSpeedDisplay;
    [SerializeField] Image playerHealthDisplay, opponentHealthDisplay, tempPlayerHealthDisplay, tempOpponentHealthDisplay;
    [SerializeField] TextMeshProUGUI playerDefenseDisplay, opponentDefenseDisplay;

    public List<GameObject> playerCardDisplays;
    [SerializeField] List<RectTransform> cardPositions;
    [SerializeField] Transform cardParent;
    [SerializeField] RectTransform playCardBorder;
    [SerializeField] RectTransform drawDeckPosition, discardDeckPosition;
    
    
    [Header("Settings")]
    [SerializeField] float speedMeterHeight;
    [SerializeField] AnimationCurve lerpCurve;
    [SerializeField] Image enemyPortrait;
    [SerializeField] List<Sprite> enemyPortraits;
    [SerializeField] List<Color> speedColors;

    [SerializeField] Sprite guidePortrait;
    [SerializeField] Color guideColor;



    public void updateSpeedUI(float opponentSpeedPercentage, float playerSpeedPercentage)
    {
        playerSpeedDisplay.setTargetPosition(playerSpeedPercentage, speedMeterHeight);
        opponentSpeedDisplay.setTargetPosition(opponentSpeedPercentage, speedMeterHeight);
    }

    public void updateHealthUI(int opponentHealth, int opponentMaxHealth, int opponentTempHealth, int playerHealth, int playerMaxHealth, int playerTempHealth)
    {
        float opponentPercentage = (float)opponentHealth/(float)opponentMaxHealth;
        float playerPercentage = (float)playerHealth/(float)playerMaxHealth;
        float tempOpponentPercentage = (float)opponentTempHealth / (float)opponentMaxHealth;
        float tempPlayerPercentage = (float)playerTempHealth / (float)playerMaxHealth;

        StopAllCoroutines();

        StartCoroutine(updateHealthBar(playerHealthDisplay, playerPercentage, 0.3f));
        StartCoroutine(updateHealthBar(opponentHealthDisplay, opponentPercentage, 0.3f));
        StartCoroutine(updateHealthBar(tempPlayerHealthDisplay, tempPlayerPercentage, 0.3f));
        StartCoroutine(updateHealthBar(tempOpponentHealthDisplay, tempOpponentPercentage, 0.3f));
    }

    public void updateDefenseUI(int opponentDefense, int playerDefense)
    {

        opponentDefenseDisplay.text = opponentDefense.ToString();
        playerDefenseDisplay.text = playerDefense.ToString();
    }

    public void setDefaultPositions()
    {
        playerSpeedDisplay.setDefaultInfo(speedMeterHeight);
        opponentSpeedDisplay.setDefaultInfo(speedMeterHeight);
    }

    public void spawnCard(card cardInfo, bool hasSpawnPosition, Vector3 spawnPosition)
    {
        GameObject newCard = null;
        switch(cardInfo.type)
        {
            case card.cardType.Attack:
                newCard = Instantiate(gameManager.instance.attackCardPrefab, cardParent);
                break;

            case card.cardType.Defend:
                newCard = Instantiate(gameManager.instance.defendCardPrefab, cardParent);
                break;

            case card.cardType.Effect:
                newCard = Instantiate(gameManager.instance.effectCardPrefab, cardParent);
                break;

            case card.cardType.Cursed:
                newCard = Instantiate(gameManager.instance.nullCardPrefab, cardParent);
                break;

            default:
                Debug.LogWarning("Invalid Card Type!!!");
                return;
        }

        TextMeshProUGUI cardDescription = newCard.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        string displayString = cardInfo.cardStrength.ToString();


        if (cardInfo.type == card.cardType.Defend && combatManager.instance.bonusPlayerDefend > 0)
        {
            displayString = "(" + displayString + "+" + combatManager.instance.bonusPlayerDefend.ToString() +")";
        } else if ((cardInfo.type == card.cardType.Attack && combatManager.instance.bonusPlayerAttack > 0) || (cardInfo.cardName == "outburst" && combatManager.instance.bonusPlayerAttack > 0))
        {
            displayString = "(" + displayString + "+" + combatManager.instance.bonusPlayerAttack.ToString() + ")";
        } 
        
        if (cardInfo.type == card.cardType.Effect || cardInfo.type == card.cardType.Cursed)
        {
            cardDescription.text = cardInfo.description;
        }

        cardDescription.text = cardDescription.text.Replace("$", displayString);

        while(true)
        {
            cardDescription.ForceMeshUpdate();
            if (cardDescription.isTextOverflowing)
            {
                cardDescription.fontSize--;
            } else
            {
                break;
            }
        }

        cardFeedback cardScript = newCard.GetComponent<cardFeedback>();
        cardScript.uiController = this;
        cardScript.cardInfo = cardInfo;
        cardScript.cardState = "hand";
        cardScript.playBorder = playCardBorder;
        
        if (!hasSpawnPosition)
        {
            cardScript.cardTransform.localPosition = drawDeckPosition.localPosition;
        } else
        {
            cardScript.cardTransform.localPosition = spawnPosition;
        }

        playerCardDisplays.Add(newCard);

        orderCards();

    }

    private void orderCards()
    {
        //place cards in hand depending on whether or not amount of cards in hand is even

        List<GameObject> playerHandDisplays = new List<GameObject>();

        for (int i = 0; i <playerCardDisplays.Count; i++)
        {
            if (playerCardDisplays[i].GetComponent<cardFeedback>().cardState == "selected") continue;

            playerHandDisplays.Add(playerCardDisplays[i]);
        }

        //layout cards
        if (playerHandDisplays.Count % 2 == 0) //even
        {
            for (int i = 0; i < playerHandDisplays.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        setCardTransform(0, 1, i, 1, playerHandDisplays);
                        break;

                    case 1:
                        setCardTransform(1, 2, i, 2, playerHandDisplays);
                        break;

                    case 2:
                        setCardTransform(2, 3, i, 3, playerHandDisplays);
                        break;

                    case 3:
                        setCardTransform(3, 4, i, 4, playerHandDisplays);
                        break;

                }
            }
        } else //odd
        {
            for (int i = 0; i < playerHandDisplays.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        setCardTransform(0, -1, i, 1, playerHandDisplays);
                        break;

                    case 1:
                        setCardTransform(1, -1, i, 2, playerHandDisplays);
                        break;

                    case 2:
                        setCardTransform(2, -1, i, 3, playerHandDisplays);
                        break;

                    case 3:
                        setCardTransform(3, -1, i, 4, playerHandDisplays);
                        break;

                    case 4:
                        setCardTransform(4, -1, i, 5, playerHandDisplays);
                        break;
                }
            }
        }
    }
     
    private void setCardTransform(int cardPositionIndex0, int cardPositionIndex1, int playerHandIndex, int sortingIndex, List<GameObject> playerHandDisplays)
    {
        RectTransform cardDisplayTransform = playerHandDisplays[playerHandIndex].GetComponent<RectTransform>();
        cardFeedback cardScript = playerHandDisplays[playerHandIndex].GetComponent<cardFeedback>();

        switch (playerHandDisplays.Count)
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
            cardScript.targetPosition = cardPosition0.localPosition;
            cardScript.setRotation(cardPosition0.rotation);
        } else //even
        {
            cardScript.targetPosition = (cardPosition0.localPosition + cardPosition1.localPosition)/2;
            cardScript.setRotation(Quaternion.Lerp(cardPosition0.rotation, cardPosition1.rotation, 0.5f));
        }

        cardDisplayTransform.SetSiblingIndex(sortingIndex * 2);
        cardScript.sortingIndex = sortingIndex * 2; 
    }

    public void selectCard(GameObject selectedCard)
    {
        //playerCardDisplays.Remove(selectedCard); 
        orderCards();
    }

    public void discardCard(GameObject discardedCard)
    {
        cardFeedback cardScript = discardedCard.GetComponent<cardFeedback>();
        cardScript.discardCard(discardDeckPosition.localPosition);

        if (discardedCard.GetComponent<cardFeedback>().cardInfo.type == card.cardType.Cursed)
        {

        }
        else
        {
            combatManager.instance.playerHand.Remove(cardScript.cardInfo);
            combatManager.instance.discardPile.Add(cardScript.cardInfo);
        }

    }
    
    public void discardAllCards()
    {
        for (int i = 0; i < playerCardDisplays.Count; i++ )
        {
            if (playerCardDisplays[i].GetComponent<cardFeedback>().cardState == "discard")
            {
                continue; 
            }

            discardCard(playerCardDisplays[i]);
        }
    }

    public void clearCards()
    {
        print(playerCardDisplays.Count);
        while(playerCardDisplays.Count > 0)
        {
            Destroy(playerCardDisplays[0]);
            playerCardDisplays.RemoveAt(0); 
        }
    }

    IEnumerator updateHealthBar(Image bar, float targetFill, float duration)
    {
        float timer = 0;
        float startFillAmount = bar.fillAmount;

        while(true)
        {
            timer += Time.deltaTime;
            float lerpValue = lerpCurve.Evaluate(timer / duration);

            bar.fillAmount = Mathf.Lerp(startFillAmount, targetFill, lerpValue);

            if (timer >= duration)
            {
                break;
            }
            yield return null;
        }
    }

    public void resetUIs()
    {
        //StartCoroutine(updateHealthBar(opponentHealthDisplay, 1, 0.1f));
        //StartCoroutine(updateHealthBar(tempOpponentHealthDisplay, 1, 0.1f));

        playerSpeedDisplay.resetPosition();
        opponentSpeedDisplay.resetPosition();
        generateEnemyPortrait();
    }

    private void generateEnemyPortrait()
    {
        if (DialogueManager.GetInstance().tutorialStage >= 4)
        {
            int index = Random.Range(0, enemyPortraits.Count);
            enemyPortrait.sprite = enemyPortraits[index];
            opponentSpeedDisplay.GetComponentsInChildren<Image>()[1].color = speedColors[index];
        }
        else
        {
            enemyPortrait.sprite = guidePortrait;
            opponentSpeedDisplay.GetComponentsInChildren<Image>()[1].color = guideColor;
        }
    }
} 
