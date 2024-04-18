using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class subwayUI : MonoBehaviour
{
    public static subwayUI instance;
    [SerializeField] nodeManager nodeManagerScript;
    [SerializeField] mapButton mapToggle;
    [SerializeField] GameObject deckParent, drawParent, discardParent;
    [SerializeField] TextMeshProUGUI followerDisplay, timeDisplay;

    [SerializeField] int defaultHour;
    private string state;

    [SerializeField] AudioSource[] audios;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            nodeManager.instance = nodeManagerScript;

            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        closeUI();
    }

    public void switchToMap()
    {
        if (state == "map")
        {
            closeUI();
            return;
        }
        mapToggle.toggleMapVisibility(true);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
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
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(true);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
        Time.timeScale = 0;

        state = "deck";
    }

    private void closeUI()
    {
        
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(false);
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
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(true);
        discardParent.SetActive(false);
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
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        drawParent.SetActive(false);
        discardParent.SetActive(true);
        Time.timeScale = 0;

        state = "discard";
    }

    public void refreshUI(int additionalTime, float timeAnimationDuration)
    {

        followerDisplay.text = gameManager.instance.followerAmount.ToString();

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
            int hour = (newTime - minute) / 60;
            timeDisplay.text = hour.ToString("00") + ":" + minute.ToString("00") + " PM";

            if (progressPercentage >= 1)
            {
                break;
            }
            yield return null;
        }
    }
}
