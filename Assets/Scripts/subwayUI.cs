using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subwayUI : MonoBehaviour
{
    public static subwayUI instance;
    [SerializeField] nodeManager nodeManagerScript;
    [SerializeField] mapButton mapToggle;
    [SerializeField] GameObject deckParent;
    private string state;

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
        Time.timeScale = 0;

        state = "deck";
    }

    private void closeUI()
    {
        
        mapToggle.toggleMapVisibility(false);
        deckParent.SetActive(false);
        Time.timeScale = 1;

        state = "closed";
    }
}
