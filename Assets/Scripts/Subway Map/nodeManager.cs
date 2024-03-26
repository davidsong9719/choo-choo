
using System.Collections.Generic;
using UnityEngine;

public class nodeManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] List<mapNode> startingNodes;
    private List<mapNode> allNodes = new List<mapNode>();

    private List<mapNode> playerStartingNodes = new List<mapNode>();
    private mapNode currentNode = null;
    private string currentLine = ""; 

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

            if (startingNodes[i].birdBeakLine) currentLine = "birdBeak";
            if (startingNodes[i].pilgrimLine) currentLine = "pilgrim";
            if (startingNodes[i].galliumLine) currentLine = "gallium";

            playerStartingNodes.Add(startingNodes[i]);
            connectNodes(currentLine, i);
        }

        //randomStart
        currentNode = playerStartingNodes[Random.Range(0, playerStartingNodes.Count)];
        currentNode.isSelected = true;

        

       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
         //   currentNode.moveForward();
        }
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
                playerStartingNodes.Add(currentNode);
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

            if (line == "birdBeak")
            {
                if (!allNodes[i].birdBeakLine) continue;
                if (allNodes[i].birdBeakConnectedNodes.Count > 1) continue;
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

            case "birdBeak":
                node.birdBeakConnectedNodes.Add(connectedNode);
                break;

            case "gallium":
                node.galliumConnectedNodes.Add(connectedNode);
                break;
        }
    }

    
}
