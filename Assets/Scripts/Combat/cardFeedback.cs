using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cardFeedback : MonoBehaviour
{
    //handles all card information, not just feedback

    [HideInInspector] public int sortingIndex;
    [HideInInspector] public combatUI uiController;
    [HideInInspector] public string cardState; //selected, hand, discard, freeze, newReplace, deckReplace, replaceSelected
    [HideInInspector] public card cardInfo;
    [HideInInspector] public RectTransform playBorder;
    [HideInInspector] public Vector2 targetPosition;
    [HideInInspector] public cardOptions newCardManager;

    [Header("Setup")]
    public RectTransform cardTransform;
    [SerializeField] Image highlightedDisplay;
    [SerializeField] Sprite highlighedSprite, unhighlightedSprite;

    [Header("Settings")]
    [SerializeField] float hoverScale;
    [SerializeField] float cardSpeed;

    private Vector3 defaultScale;
    private Quaternion defaultRotation;
    

    private Vector3 refVelocity; //for smoothdamp

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (cardState == "hand" || cardState == "discard")
        {
            moveToTargetPosition(); 
        }

        if (cardState == "selected")
        {
            followMouse();


            if (cardTransform.localPosition.y > playBorder.localPosition.y) //can Play
            {
                highlight();
            }
            else //return to hand
            {
                unhighlight();
            }

        }

        if (cardState == "discard")
        {
            if (Vector2.Distance(cardTransform.localPosition, targetPosition) < 50f)
            {
                uiController.playerCardDisplays.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    public void onHoverEnter()
    {
        if (cardState == "freeze") return;
        transform.SetAsLastSibling();
        transform.localScale = defaultScale * hoverScale;
    }

    public void onHoverExit()
    {
        if (cardState == "freeze") return;
        transform.SetSiblingIndex(sortingIndex);
        transform.localScale = defaultScale;
    }

    public void onClick()
    {
        if (cardState == "hand")
        {
            if (!uiController.hasSelectedCard)
            {
                uiController.hasSelectedCard = true;
                Invoke(nameof(selectCard), 0);
            }
        }
            
        if (cardState == "selected")
        {
            if (cardTransform.localPosition.y > playBorder.localPosition.y) //play
            {
               cardState = "freeze";
               combatManager.instance.startPlayCard(gameObject, cardInfo);
            } else //return to hand
            {
                Invoke(nameof(returnCard), 0);
            }
        }

        if (cardState == "newReplace")
        {
            newCardManager.selectNewCard(this);
        }

        if (cardState == "deckReplace")
        {
            newCardManager.selectDeckCard(this);
        }
    }

    private void followMouse()
    {
        transform.position = Input.mousePosition;
    }

    public void setRotation(Quaternion rotation)
    {
        cardTransform.rotation = rotation;
        defaultRotation = rotation;
    }

    private void selectCard()
    {
        cardState = "selected";
        //print("select");
        uiController.selectCard(gameObject);
        cardTransform.rotation = Quaternion.identity;
        transform.SetAsLastSibling();
    }

    private void returnCard() //to hand
    {
        uiController.hasSelectedCard = false;
        uiController.playerCardDisplays.Remove(gameObject);
        uiController.spawnCard(cardInfo, true, cardTransform.localPosition);
        Destroy(gameObject);
    }
    public void moveToTargetPosition()
    {
        cardTransform.localPosition = Vector3.SmoothDamp(cardTransform.localPosition, targetPosition, ref refVelocity, cardSpeed); 
    }

    public void discardCard(Vector3 discardDeckPosition)
    {
        cardState = "discard";
        targetPosition = discardDeckPosition;
    }

    public void highlight()
    {
        highlightedDisplay.sprite = highlighedSprite;
    }

    public void unhighlight()
    {
        highlightedDisplay.sprite = unhighlightedSprite;
    }
}
