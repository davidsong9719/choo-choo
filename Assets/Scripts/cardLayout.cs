using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cardLayout : MonoBehaviour
{
    private enum displayInfo
    {
        playerDeck,
        drawPile,
        discardPile,
        exchangeDeck
    }

    private List<card> cards = new List<card>();

    [Header("Setup")]
    public List<Image> displayObject;

    [Header("Settings")]
    [SerializeField] displayInfo displayContext;

    private void OnEnable()
    {
        refreshDisplay();
    }
    public void refreshDisplay()
    {

        cards.Clear();

        switch (displayContext)
        {
            case displayInfo.playerDeck:
            case displayInfo.exchangeDeck:
                cards = copyToCardList(gameManager.instance.playerDeck, false);
                break;

            case displayInfo.drawPile:
                cards = copyToCardList(combatManager.instance.drawPile, true);
                break;

            case displayInfo.discardPile:
                cards = copyToCardList(combatManager.instance.discardPile, true);
                break;
        }

        for (int i = 0; i < displayObject.Count; i++)
        {
            displayObject[i].enabled = false;
            if (i >= cards.Count)
            {
                generateCard(displayObject[i].transform, null);
            } else
            {
                cardFeedback cardScript =  generateCard(displayObject[i].transform, cards[i]).GetComponent<cardFeedback>();

                if (displayContext == displayInfo.exchangeDeck)
                {
                    cardScript.cardState = "deckReplace";
                    cardScript.newCardManager = GetComponent<cardOptions>();
                } 
            }
        }
    }

    private List<card> copyToCardList(List<card> targetList, bool shuffle)
    {
        List<card> newList = new List<card>();

        for (int i = 0; i < targetList.Count; i++)
        {
            newList.Add(targetList[i]);
        }

        if (!shuffle) return newList;

        List<card> shuffledNewList = new List<card>();

        while (newList.Count > 0)
        {
            int randomIndex = Random.Range(0, newList.Count);
            shuffledNewList.Add(newList[randomIndex]);
            newList.RemoveAt(randomIndex);
        }

        return shuffledNewList;
    }

    public GameObject generateCard(Transform parent, card cardInfo)
    {
        if (parent.childCount > 0)
        {
            Destroy(parent.GetChild(0).gameObject);
        }

        GameObject newCard = null;

        if (cardInfo == null)
        {
            newCard = Instantiate(gameManager.instance.nullCardPrefab, parent);
            return newCard;
        }

        switch (cardInfo.type)
        {
            case card.cardType.Attack:
                newCard = Instantiate(gameManager.instance.attackCardPrefab, parent);
                break;

            case card.cardType.Defend:
                newCard = Instantiate(gameManager.instance.defendCardPrefab, parent);
                break;

            case card.cardType.Effect:
                switch (cardInfo.cardName)
                {
                    case "redraw":
                        newCard = Instantiate(gameManager.instance.effectCardPrefab, parent);
                        break;

                    case "increaseAttack":
                    case "lifeSteal":
                    case "outburst":
                        newCard = Instantiate(gameManager.instance.pulseCardPrefab, parent);
                        break;

                    case "increaseDefend":
                    case "curse":
                    case "chainRetort":
                        newCard = Instantiate(gameManager.instance.galliumCardPrefab, parent);
                        break;

                    case "increaseHand":
                    case "pray":
                    case "confuse":
                        newCard = Instantiate(gameManager.instance.pilgrimCardPrefab, parent);
                        break;

                }
                break;
        }

        newCard.GetComponent<cardFeedback>().cardInfo = cardInfo;

        TextMeshProUGUI cardDescription = newCard.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        string displayString = cardInfo.cardStrength.ToString();


        if (cardInfo.type == card.cardType.Defend && combatManager.instance.bonusPlayerDefend > 0)
        {
            displayString = "(" + displayString + "+" + combatManager.instance.bonusPlayerDefend.ToString() + ")";
        }
        else if (cardInfo.type == card.cardType.Attack && combatManager.instance.bonusPlayerAttack > 0)
        {
            displayString = "(" + displayString + "+" + combatManager.instance.bonusPlayerAttack.ToString() + ")";
        }
        else if (cardInfo.type == card.cardType.Effect)
        {
            cardDescription.text = cardInfo.description;
        }

        cardDescription.text = cardDescription.text.Replace("$", displayString);

        return newCard;

    }
}
