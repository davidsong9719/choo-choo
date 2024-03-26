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
    [HideInInspector] public bool isSelected;
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
        if (isSelected)
        {
            highlightAnimation();
        }
    }

    private void highlightAnimation()
    {
        if (animationDirection)
        {
            highlightImage.color = Color.Lerp(Color.white, Color.black, animationCounter += Time.deltaTime*2f);
        }

        if (!animationDirection)
        {
            highlightImage.color = Color.Lerp(Color.black, Color.white, animationCounter += Time.deltaTime*2f);
        }

        if (animationCounter >= 2)
        {
            animationDirection = !animationDirection;
            animationCounter = 0;
        }

        
    }

    public mapNode moveForward(string line)
    {
        mapNode newPosition = null;

        switch (line)
        {
            case "pilgrim":
                newPosition = moveNodes(pilgrimConnectedNodes, 1);
                break;

            case "birdBeak":
                newPosition = moveNodes(birdBeakConnectedNodes, 1); 
                break;

            case "gallium":
                newPosition = moveNodes(galliumConnectedNodes, 1);
                break;
        }

        return newPosition;
    }

    public mapNode moveBackward(string line)
    {
        mapNode newPosition = null;
        
        switch (line)
        {
            case "pilgrim":
                newPosition = moveNodes(pilgrimConnectedNodes, 0);
                break;

            case "birdBeak":
                newPosition = moveNodes(birdBeakConnectedNodes, 0);
                break;

            case "gallium":
                newPosition = moveNodes(galliumConnectedNodes, 0);
                break;
        }

        
        return newPosition;
    }

    private mapNode moveNodes(List<mapNode> lineNodes, int direction)
    {
        isSelected = false;
        lineNodes[direction].isSelected = true;

        return lineNodes[direction];
    }
}
