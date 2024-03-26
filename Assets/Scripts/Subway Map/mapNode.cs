using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class mapNode : MonoBehaviour
{
    [HideInInspector] public Vector2 anchoredPosition;
    [HideInInspector] public bool isCurrent;
    public List<mapNode> pilgrimConnectedNodes, birdBeakConnectedNodes, galliumConnectedNodes;

    [Header("Settings")]
    public bool pilgrimLine;
    public bool birdBeakLine;
    public bool galliumLine;

    private Image highlightImage;
    private bool animationDirection;
    private float animationCounter;

    private void Awake()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        highlightImage = transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        if (isCurrent)
        {
            highlightAnimation();
        }
    }

    private void highlightAnimation()
    {
        if (animationDirection)
        {
            highlightImage.color = Color.Lerp(Color.white, Color.black, animationCounter += Time.unscaledDeltaTime*2f);
        }

        if (!animationDirection)
        {
            highlightImage.color = Color.Lerp(Color.black, Color.white, animationCounter += Time.unscaledDeltaTime * 2f);
        }

        if (animationCounter >= 2)
        {
            animationDirection = !animationDirection;
            animationCounter = 0;
        }

        
    }

    public mapNode moveNode(string line, int direction)
    {
        mapNode newPosition = null;

        switch (line)
        {
            case "pilgrim":
                newPosition = moveNodes(pilgrimConnectedNodes, direction);
                break;

            case "birdBeak":
                newPosition = moveNodes(birdBeakConnectedNodes, direction); 
                break;

            case "gallium":
                newPosition = moveNodes(galliumConnectedNodes, direction);
                break;
        }

        return newPosition;
    }

    private mapNode moveNodes(List<mapNode> lineNodes, int direction)
    {
        toggleCurrent(false);
        lineNodes[direction].toggleCurrent(true);

        return lineNodes[direction];
    }
    public void toggleCurrent(bool isCurrentNode)
    {
        isCurrent = isCurrentNode;

        if (isCurrent)
        {
            highlightImage.color = Color.black;
        } else
        {
            highlightImage.color = Color.white;
        }

        animationCounter = 0;
    }
}
