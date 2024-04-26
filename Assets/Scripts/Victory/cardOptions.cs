using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardOptions : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] List<Transform> displayObject;
    [SerializeField] RectTransform deckParent, newCardsParent;
    [SerializeField] Image replaceButton, continueButton, background;
    [SerializeField] Vector3 deckXSET, newCardsXSET; //x axis start, end, and lerp time
    private cardLayout layoutManager;
    private cardFeedback selectedNewCard, selectedDeckCard;

    private bool isReplaceable, isContinuable;

    private void Awake()
    {
        layoutManager = GetComponent<cardLayout>();
        replaceButton.color = Color.gray;
        continueButton.color = Color.gray;
    }

    private void OnEnable()
    {
        replaceButton.color = Color.gray;
        continueButton.color = Color.gray;
        isContinuable = false;
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

        StartCoroutine(transition(newCardsParent, newCardsXSET, true));
        StartCoroutine(transition(deckParent, deckXSET, true));
        StartCoroutine(fadeIn(background, 0.5f));
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

        if (DialogueManager.GetInstance().tutorialStage == 2)
        {
            DialogueManager.GetInstance().tutorialStage = 3;
            combatManager.instance.startCombat();
            subwayUI.instance.setGuideTextPerm("Board a train by the stairs");
        }
        else
        {
            subwayManager.instance.switchToMovement();
            subwayUI.instance.setGuideTextPerm("");
        }

        if (DialogueManager.GetInstance().tutorialStage > 3 && DialogueManager.GetInstance().result == "")
        {
            nodeManager.instance.progressStation();
            combatManager.instance.npcManagerScript.updateCar();
        }
    }

    IEnumerator transition(RectTransform movedObject, Vector3 startEndTime, bool isMovingX)
    {
        float timeCounter = -Time.deltaTime; // to zero out the value for the first loop

        while (true)
        {
            timeCounter += Time.deltaTime;

            float newPosition = Mathf.Lerp(startEndTime.x, startEndTime.y, gameManager.instance.lerpCurve.Evaluate(timeCounter / startEndTime.z));

            if (isMovingX)
            {
                movedObject.anchoredPosition = new Vector2(newPosition, movedObject.anchoredPosition.y);
            }
            else
            {
                movedObject.anchoredPosition = new Vector2(movedObject.anchoredPosition.x, newPosition);
            }

            if (timeCounter >= startEndTime.z) break;
            yield return null;
        }

    }

    IEnumerator fadeIn(Image image, float fadeTime)
    {
        float timeCounter = -Time.deltaTime;
        while (true)
        {
            timeCounter += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, gameManager.instance.lerpCurve.Evaluate(timeCounter / fadeTime));

            if (timeCounter >= fadeTime) break;
            yield return null;
        }
    }
}
