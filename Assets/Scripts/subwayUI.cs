using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class subwayUI : MonoBehaviour
{
    public static subwayUI instance;
    [SerializeField] nodeManager nodeManagerScript;
    [SerializeField] mapButton mapToggle;
    [SerializeField] GameObject deckParent, drawParent, discardParent, lineMenuParent, healthDisplay, combatHealthParent, uiHealthParent, pauseParent;
    [SerializeField] TextMeshProUGUI followerDisplay, timeDisplay, guideText;
    [SerializeField] int defaultHour;
    public string state = "closed";
    private string permText;
    [SerializeField] int fontDefaultSize, fontUpdateSize;
    [SerializeField] Vector3 stationChoiceYSET;
    private Coroutine timeCoroutine, guideCoroutine;

    private string prePauseState, prePausePermText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            nodeManager.instance = nodeManagerScript;

            //DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        closeUI();
    }

    private string createMapPermText()
    {
        string mapText = ", heading towards ";
        switch (nodeManager.instance.currentLine)
        {
            case "pulse":
                if (nodeManager.instance.currentDirection == 0)
                {
                    mapText = "Pulse Line" + mapText + "West Station";
                }
                else
                {
                    mapText = "Pulse Line" + mapText + "South Station";
                }
                break;

            case "pilgrim":
                if (nodeManager.instance.currentDirection == 0)
                {
                    mapText = "Pilgrim Line" + mapText + "West Station";
                }
                else
                {
                    mapText = "Pilgrim Line" + mapText + "East Station";
                }
                break;

            case "gallium":
                if (nodeManager.instance.currentDirection == 0)
                {
                    mapText = "Gallium Line" + mapText + "South Station";
                }
                else
                {
                    mapText = "Gallium Line" + mapText + "North Station";
                }
                break;

            case "null":
                mapText = "";
                break;

        }

        return mapText;
    }

    public void switchToMap()
    {
        if (state == "map")
        {
            closeUI();
            return;
        }

        setGuideTextPerm(createMapPermText());
        mapToggle.toggleMapVisibility(true);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
        lineMenuParent.SetActive(false);
        
        Time.timeScale = 0;


        state = "map";
    }

    public void switchToDeck()
    {
        if (state == "deck")
        {
            closeUI();
            return;
        }

        setGuideTextPerm("The Deck");
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(true);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
        lineMenuParent.SetActive(false);
        Time.timeScale = 0;

        state = "deck";
    }

    public void closeUI()
    {
        setGuideTextPerm("");
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
        lineMenuParent.SetActive(false);
        Time.timeScale = 1;

        state = "closed";
    }

    public void switchToDraw()
    {
        if (state == "draw")
        {
            closeUI();
            return;
        }

        setGuideTextPerm("The Draw Deck");
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(true);
        discardParent.SetActive(false);
        lineMenuParent.SetActive(false);
        Time.timeScale = 0;

        state = "draw";
    }

    public void switchtoDiscard()
    {
        if (state == "discard")
        {
            closeUI();
            return;
        }

        setGuideTextPerm("The Discard Deck");
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(true);
        lineMenuParent.SetActive(false);
        Time.timeScale = 0;

        state = "discard";
    }



    public void switchToLineMenu()
    {
        if (state == "line")
        {
            StartCoroutine(lineTransition(lineMenuParent.GetComponent<RectTransform>(), new Vector3(stationChoiceYSET.y, stationChoiceYSET.x, stationChoiceYSET.z), true));
            return;
        }
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
        lineMenuParent.SetActive(true);
        Time.timeScale = 0;
        state = "line";

        StartCoroutine(lineTransition(lineMenuParent.GetComponent<RectTransform>(), stationChoiceYSET, false));
    }

    IEnumerator lineTransition(RectTransform movedObject, Vector3 startEndTime, bool isClosing)
    {
        float timeCounter = -Time.unscaledDeltaTime; // to zero out the value for the first loop

        while (true)
        {
            timeCounter += Time.unscaledDeltaTime;

            float newPosition = Mathf.Lerp(startEndTime.x, startEndTime.y, gameManager.instance.lerpCurve.Evaluate(timeCounter / startEndTime.z));

            movedObject.anchoredPosition = new Vector2(movedObject.anchoredPosition.x, newPosition);

            if (timeCounter >= startEndTime.z)
            {
                if (isClosing)
                {
                    closeUI();
                }
                break;
            }
            yield return new WaitForSecondsRealtime(0.001f);
        }
    }
    public void switchToPause()
    {
        if (state == "pause")
        {
            pauseParent.SetActive(false);
            state = prePauseState;
            setGuideTextPerm(prePausePermText);
            Time.timeScale = 1;
            
            return;
        }

        pauseParent.SetActive(true);
        prePausePermText = permText;
        setGuideTextPerm("Paused");

        Time.timeScale = 0;
        state = "pause";
    }

    public void refreshUI(int additionalTime, float timeAnimationDuration)
    {

        followerDisplay.text = gameManager.instance.followerAmount.ToString();
        StartCoroutine(updateFontSize(followerDisplay));

        StartCoroutine(updateTime(additionalTime, timeAnimationDuration));
    }

    IEnumerator updateTime(int additionalTime, float animationDuration)
    {
        float startTime = gameManager.instance.timeElapsed;
        gameManager.instance.timeElapsed +=additionalTime;

        float countTime = 0;
        while(true)
        {
            countTime += Time.deltaTime;
            float progressPercentage = countTime / animationDuration;
            int newTime = (int)Mathf.Lerp(startTime, startTime + additionalTime, progressPercentage);

            int minute = newTime % 60;
            int hour = ((newTime - minute) / 60)+defaultHour;
            timeDisplay.text = hour.ToString("00") + ":" + minute.ToString("00") + " PM";

            if (timeCoroutine != null)
            {
                StopCoroutine(timeCoroutine);
            }

            guideCoroutine = StartCoroutine(updateFontSize(timeDisplay));

            if (progressPercentage >= 1)
            {
                break;
            }
            yield return null;
        }

        if (gameManager.instance.timeElapsed >= gameManager.instance.stageOneLength + gameManager.instance.stageTwoLength + gameManager.instance.stageThreeLength)
        {
            DialogueManager.GetInstance().tutorialStage = 5;

            while (true)
            {
                if (TransitionManager.GetInstance().playingTransition == false)
                {
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            yield return null;
            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToStation));
            DialogueManager.GetInstance().result = "";
        }

    }

    public void setGuideTextTemp(string newText)
    {
        guideText.text = newText;
        if (guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
        }
        guideCoroutine = StartCoroutine(updateFontSize(guideText));
    }

    public void setGuideTextPerm(string newText)
    {
        if (guideText.text == permText)
        {
            permText = newText;
            setGuideTextToPerm();
        } else
        {
            permText = newText;
        }
    }

    public void setGuideTextToPerm()
    {
        guideText.text = permText;
        if (guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
        }
        
        guideCoroutine = StartCoroutine(updateFontSize(guideText));
    }

    IEnumerator updateFontSize(TextMeshProUGUI textComponent)
    {
        float resetTime = 0.2f;
        float timeCounter = 0;
        float lerpCurveValue = 0;

        while (true)
        {
            timeCounter += Time.deltaTime;
            lerpCurveValue = gameManager.instance.lerpCurve.Evaluate(timeCounter / resetTime);

            textComponent.fontSize = Mathf.Lerp(fontUpdateSize, fontDefaultSize, lerpCurveValue);

            if (timeCounter > resetTime) break;
            yield return null;
        }
    }

    public void startUpdateMapSize()
    {
        StartCoroutine(updateSize(mapToggle.GetComponent<RectTransform>()));
    }
    IEnumerator updateSize(RectTransform transformComponent)
    {
        float resetTime = 0.2f;
        float timeCounter = 0;
        float lerpCurveValue = 0;
        float startScale = transformComponent.localScale.x;

        while (true)
        {
            timeCounter += Time.unscaledDeltaTime;
            lerpCurveValue = gameManager.instance.lerpCurve.Evaluate(timeCounter / resetTime);
            float newScaleValue = Mathf.Lerp(startScale + 0.14f, startScale, lerpCurveValue);

            transformComponent.localScale = Vector3.one * newScaleValue;

            if (timeCounter > resetTime) break;
            yield return new WaitForSecondsRealtime(0);
        }
    }

    public void switchHealth(bool isCombat)
    {
        if (isCombat)
        {
            healthDisplay.transform.SetParent(combatHealthParent.transform);
            
        } else
        {
            healthDisplay.transform.SetParent(uiHealthParent.transform);
        }
    }
}
