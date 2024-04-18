using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardOptions : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] List<Transform> displayObject;
    [SerializeField] Image replaceButton, continueButton;

    private cardLayout layoutManager;
    private cardFeedback selectedNewCard, selectedDeckCard;

    private bool isReplaceable, isContinuable;

    

    private void Awake()
    {
        layoutManager = GetComponent<cardLayout>();
        replaceButton.color = Color.gray;
        continueButton.color = Color.gray;
    }

    public void spawnNewCards(float difficulty)
    {
        for (int i = 0; i < displayObject.Count; i++)
        {
            card newCard = cardGenerator.instance.generateNewCard(difficulty);

            cardFeedback cardScript = layoutManager.generateCard(displayObject[i], newCard).GetComponent<cardFeedback>();

            cardScript.cardState = "newReplace";
            cardScript.newCardManager = this;
        }
    }

    public void selectNewCard(cardFeedback selected)
    {
        if (selectedNewCard != null)
        {
            selectedNewCard.unhighlight();
        }

        selectedNewCard = selected;
        selectedNewCard.highlight();

        checkSelected();
    }

    public void selectDeckCard(cardFeedback selected)
    {
        if (selectedDeckCard != null)
        {
            selectedDeckCard.unhighlight();
        }

        selectedDeckCard = selected;
        selectedDeckCard.highlight();

        checkSelected();
    }

    private void checkSelected()
    {
        if (selectedNewCard != null && selectedDeckCard != null)
        {
            isReplaceable = true;
            replaceButton.color = Color.white;
        } 
    }

    public void replaceCard()
    {
        if (!isReplaceable) return;

        int index = layoutManager.displayObject.IndexOf(selectedDeckCard.transform.parent.gameObject.GetComponent<Image>());
        gameManager.instance.playerDeck[index] = selectedNewCard.cardInfo;
        Destroy(selectedNewCard.gameObject);

        layoutManager.refreshDisplay();
        isReplaceable = false;
        replaceButton.color = Color.gray;
        isContinuable = true;
        continueButton.color = Color.white;
    }

    public void continueGame()
    {
        if (!isContinuable) return;

        if (DialogueManager.GetInstance().tutorialStage == 1)
        {
            DialogueManager.GetInstance().tutorialStage = 2;
            combatManager.instance.startCombat();
        }
        else
        {
            subwayManager.instance.switchToMovement();
        }
    }
}
