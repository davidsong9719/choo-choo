using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedDisplays : MonoBehaviour
{
    private float targetY;
    private Vector2 defaultPosition;
    public float speedMeterHeight;
    [HideInInspector]
    [Header("Setup")]
    [SerializeField] RectTransform rectTransform;

    public void setTargetPosition(float speedPercentage, float meterHeight)
    {
        targetY = Mathf.Lerp(defaultPosition.y, defaultPosition.y - meterHeight, speedPercentage);
        StopCoroutine(moveDisplay(combatManager.instance.speedIncrementSpeed/2, speedPercentage == 1));
        StartCoroutine(moveDisplay(combatManager.instance.speedIncrementSpeed/2, speedPercentage == 1));
    }

    public void setDefaultInfo(float meterHeight)
    {
        defaultPosition = rectTransform.anchoredPosition;
        speedMeterHeight = meterHeight;
    }

    public void resetPosition()
    {
        rectTransform.anchoredPosition = defaultPosition;
    }

    IEnumerator moveDisplay(float duration, bool reachedEnd)
    {
        yield return null;
        float startingY = rectTransform.anchoredPosition.y;
        float time = 0;

        
        while (true)
        {
            time += Time.deltaTime;
            
            float t = time / duration;

            rectTransform.anchoredPosition = new Vector2(defaultPosition.x, Mathf.SmoothStep(startingY, targetY, t));

            if (t >= 1)
            {
                break;
            }
            yield return null;
        }
        
    }
}
