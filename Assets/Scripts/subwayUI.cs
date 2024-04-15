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

    public void refreshUI()
    {

        followerDisplay.text = gameManager.instance.followerAmount.ToString();

        int elapsedTime = gameManager.instance.timeElapsed;
        int minute = elapsedTime % 60;
        int hour = (elapsedTime-minute) / 60;
        hour += defaultHour;

        timeDisplay.text = hour.ToString("00") + ":" + minute.ToString("00") + " PM";   
    }
}
