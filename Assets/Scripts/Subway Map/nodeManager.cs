
using System.Collections.Generic;
using UnityEngine;

public class nodeManager : MonoBehaviour
{
    //limitations: the ends must not have multiple lines
    //
    //

    public static nodeManager instance;

    [Header("Setup")]
    [SerializeField] List<mapNode> startingNodes;
    private List<mapNode> allNodes = new List<mapNode>(); //only one end of each line

    private List<mapNode> allStartingNodes = new List<mapNode>(); //both ends of each line
    public mapNode currentNode = null;
    public string currentLine = "";
    public int currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <transform.childCount; i++)
        {
            mapNode currentNode = transform.GetChild(i).GetComponent<mapNode>();
            allNodes.Add(currentNode);
        }

        for (int i = 0; i < startingNodes.Count; i++)
        {
            string currentLine = "";

            if (startingNodes[i].pulseLine) currentLine = "pulse";
            if (startingNodes[i].pilgrimLine) currentLine = "pilgrim";
            if (startingNodes[i].galliumLine) currentLine = "gallium";

            allStartingNodes.Add(startingNodes[i]);
            connectNodes(currentLine, i);
        }

        //randomStart
        currentNode = allStartingNodes[Random.Range(0, allStartingNodes.Count)];
        currentNode.toggleCurrent(true);

        if (currentNode.pulseLine) currentLine = "pulse";
        if (currentNode.pilgrimLine) currentLine = "pilgrim";
        if (currentNode.galliumLine) currentLine = "gallium";

        if (startingNodes.Contains(currentNode))
        {
            currentDirection = 1;
        } else
        {
            currentDirection = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            progressStation();
        }
    }

    public void progressStation()
    {
        currentNode = currentNode.moveNode(currentLine, currentDirection);
    } 

    private void connectNodes(string line, int startingNodeIndex)
    {
        //mapNode previousNode;
        mapNode currentNode = startingNodes[startingNodeIndex];
        addNodeToConnectedList(currentNode, line, currentNode);

        mapNode nextNode;

        while (true)
        {
            nextNode = findClosestNode(currentNode, line);
            if (nextNode == null)
            {
                allStartingNodes.Add(currentNode);
                addNodeToConnectedList(currentNode, line, currentNode);
                break;
            }

            addNodeToConnectedList(currentNode, line, nextNode);
            addNodeToConnectedList(nextNode, line, currentNode);

            currentNode = nextNode;
        }
    }

    private mapNode findClosestNode(mapNode currentNode, string line)
    {
        mapNode closestNode = null;

        for (int i = 0; i < allNodes.Count; i++)
        {
            //checks
            if (allNodes[i] == currentNode) continue;

            if (line == "pilgrim")
            {
                if (!allNodes[i].pilgrimLine) continue;
                if (allNodes[i].pilgrimConnectedNodes.Count > 1) continue;
            }

            if (line == "pulse")
            {
                if (!allNodes[i].pulseLine) continue;
                if (allNodes[i].pulseConnectedNodes.Count > 1) continue;
            }

            if (line == "gallium")
            {
                if (!allNodes[i].galliumLine) continue;
                if (allNodes[i].galliumConnectedNodes.Count > 1) continue;
            }

            if (closestNode == null)
            {
                closestNode = allNodes[i];
                continue;
            }

            //set values
            float nodeDistance = Vector3.Distance(currentNode.anchoredPosition, allNodes[i].anchoredPosition);
            float currentClosestDistance = Vector3.Distance(currentNode.anchoredPosition, closestNode.anchoredPosition);

            if (nodeDistance < currentClosestDistance)
            {
                closestNode = allNodes[i];
            }
        }
        
        return closestNode;
    }

    private void addNodeToConnectedList(mapNode node, string line, mapNode connectedNode)
    {
        switch(line)
        {
            case "pilgrim":
                node.pilgrimConnectedNodes.Add(connectedNode);
                break;

            case "pulse":
                node.pulseConnectedNodes.Add(connectedNode);
                break;

            case "gallium":
                node.galliumConnectedNodes.Add(connectedNode);
                break;
        }
    }



    
}
