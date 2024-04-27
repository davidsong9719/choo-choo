using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapNode : MonoBehaviour
{
    [HideInInspector] public Vector2 anchoredPosition;
    [HideInInspector] public bool isCurrent;
    public List<mapNode> pilgrimConnectedNodes, pulseConnectedNodes, galliumConnectedNodes;

    [Header("Settings")]
    public bool pilgrimLine;
    public bool pulseLine;
    public bool galliumLine;

    private void Awake()
    {
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public mapNode moveNode(string line, int direction)
    {
        mapNode newPosition = null;

        switch (line)
        {
            case "pilgrim":
                newPosition = moveNodes(pilgrimConnectedNodes, direction);
                break;

            case "pulse":
                newPosition = moveNodes(pulseConnectedNodes, direction); 
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
    }
}
