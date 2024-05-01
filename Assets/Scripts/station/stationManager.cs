using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class stationManager : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] List<Transform> menuLines;
    [SerializeField] nodeManager mapManager;

    [SerializeField] GameObject cultGuide;

    [SerializeField] Material trainColors;
    [SerializeField] Texture pulseColors;
    [SerializeField] Texture pilgrimColors;
    [SerializeField] Texture galliumColors;

    [Header("Logo Icons")]
    [SerializeField] Sprite pilgrimSprite;
    [SerializeField] Sprite pulseSprite;
    [SerializeField] Sprite galliumSprite;
    [SerializeField] Sprite pilgrimHighlight, pulseHighlight, galliumHightlight;
    private struct lineInfo
    {
        public int direction;
        public string line;
        public int time;
        public lineInfo(int direction, string line, int time)
        {
            this.direction = direction;
            this.line = line;
            this.time = time;
        }
    }
    private Dictionary<Transform, lineInfo> lineInfoPairs = new Dictionary<Transform, lineInfo>();


    public void startStation()
    {
        if (subwayManager.instance.state != "start")
        {
            GameObject player = subwayManager.instance.player;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = playerSpawnPosition.position;
            player.transform.rotation = playerSpawnPosition.rotation;
            player.GetComponent<CharacterController>().enabled = true;
        }

        if (DialogueManager.GetInstance().tutorialStage == 4)
        {
            cultGuide.SetActive(false);
        }

        else if (DialogueManager.GetInstance().tutorialStage == 5)
        {
            cultGuide.SetActive(true);
        }
    }

    public void generateMenu()
    {
        lineInfoPairs.Clear();
        subwayUI.instance.switchToLineMenu();
        int index = 0;

        //display current direction
        mapNode currentNode = mapManager.currentNode;
        int currentDirection = mapManager.currentDirection;
        int oppositeDirection = 0;

        if (currentDirection == 0) oppositeDirection = 1;
        else if (currentDirection == 1) oppositeDirection = 0;

        List<mapNode> currentLineNodes = new List<mapNode>();
        List<mapNode> switchLineNodes = new List<mapNode>();
        string switchLineName = "";

        switch (mapManager.currentLine)
        {
            case "pulse":
                currentLineNodes = copyFromList(currentNode.pulseConnectedNodes);
                if (currentNode.pilgrimConnectedNodes.Count > 0)
                {
                    switchLineName = "pilgrim";
                    switchLineNodes = copyFromList(currentNode.pilgrimConnectedNodes);
                }
                else if (currentNode.galliumConnectedNodes.Count > 0)
                {
                    switchLineName = "gallium";
                    switchLineNodes = copyFromList(currentNode.galliumConnectedNodes);
                }
                break;
            case "pilgrim":
                currentLineNodes = copyFromList(currentNode.pilgrimConnectedNodes);
                if (currentNode.pulseConnectedNodes.Count > 0)
                {
                    switchLineName = "pulse";
                    switchLineNodes = copyFromList(currentNode.pulseConnectedNodes);
                }
                else if (currentNode.galliumConnectedNodes.Count > 0)
                {
                    switchLineName = "gallium";
                    switchLineNodes = copyFromList(currentNode.galliumConnectedNodes);
                }
                break;
            case "gallium":
                currentLineNodes = copyFromList(currentNode.galliumConnectedNodes);
                if (currentNode.pilgrimConnectedNodes.Count > 0)
                {
                    switchLineName = "pilgrim";
                    switchLineNodes = copyFromList(currentNode.pilgrimConnectedNodes);
                }
                else if (currentNode.pulseConnectedNodes.Count > 0)
                {
                    switchLineName = "pulse";
                    switchLineNodes = copyFromList(currentNode.pulseConnectedNodes);
                }
                break;
        }

        if (currentLineNodes[currentDirection] == currentNode)
        {

        }
        else
        {
            displayLine(index, mapManager.currentLine, currentDirection, currentNode);
            index++;
        }


        if (currentLineNodes[oppositeDirection] == currentNode)
        {

        }
        else
        {
            displayLine(index, mapManager.currentLine, oppositeDirection, currentNode);
            index++;
        }

        if (switchLineNodes.Count > 0)
        {
            displayLine(index, switchLineName, currentDirection, currentNode);
            index++;
            displayLine(index, switchLineName, oppositeDirection, currentNode);
            index++;
        }


        for (int i = index; i < menuLines.Count; i++)
        {
            displayLine(i, "null", 0, currentNode);
        }
    }

    private List <mapNode> copyFromList(List<mapNode> referenceList)
    {
        List <mapNode> newList = new List <mapNode>();

        for (int i = 0; i < referenceList.Count; i++)
        {
            newList.Add(referenceList[i]);
        }

        return newList;
    }

    private void displayLine(int index, string line, int direction, mapNode node)
    {
        int time = Random.Range(2, 11);

        TextMeshProUGUI nameTMP = menuLines[index].Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeTMP = menuLines[index].Find("Time").GetComponent<TextMeshProUGUI>();
        Image imageComponent = menuLines[index].Find("Logo").GetComponent<Image>();
        GameObject background = menuLines[index].Find("Background").gameObject;

        background.SetActive(false);

        if (line != "null")
        {
            lineInfoPairs.Add(menuLines[index], new lineInfo(direction, line, time));
            timeTMP.text = time.ToString() + " min";
            imageComponent.color = new Color(1, 1, 1, 1);
        } else
        {
            timeTMP.text = "";
            imageComponent.color = new Color(1, 1, 1, 0);
        }

        //display
        switch (line)
        {
            case "pulse":
                nameTMP.text = "Pulse Line - ";
                imageComponent.sprite = pulseSprite;

                if (direction == 0)
                {
                    nameTMP.text += "West Station";
                } else
                {
                    nameTMP.text += "South Station";
                }
                break;

            case "pilgrim":
                nameTMP.text = "Pilgrim Line - ";
                imageComponent.sprite = pilgrimSprite;

                if (direction == 0)
                {
                    nameTMP.text += "West Station";
                }
                else
                {
                    nameTMP.text += "East Station";
                }
                break;

            case "gallium":
                nameTMP.text = "Gallium Line - ";
                imageComponent.sprite = galliumSprite;

                if (direction == 0)
                {
                    nameTMP.text += "South Station";
                }
                else
                {
                    nameTMP.text += "North Station";
                }
                break;

            case "null":
                nameTMP.text = "";
                break;

        }
    }

    public void highlightLine(Transform callingObject)
    {
        lineInfo highlightedLine = new lineInfo();
        if (lineInfoPairs.ContainsKey(callingObject))
        {
            highlightedLine = lineInfoPairs[callingObject];
        }
        else
        {
            return;
        }

        Image imageComponent = callingObject.Find("Logo").GetComponent<Image>();
        GameObject background = callingObject.Find("Background").gameObject;
        background.SetActive(true);

        switch (highlightedLine.line)
        {
            case "pilgrim":
                imageComponent.sprite = pilgrimHighlight;
                break;

            case "gallium":
                imageComponent.sprite = galliumHightlight;
                break;

            case "pulse":
                imageComponent.sprite = pulseHighlight;
                break;
        }
    }

    public void dehighlightLine(Transform callingObject)
    {
        lineInfo highlightedLine = new lineInfo();

        if (lineInfoPairs.ContainsKey(callingObject))
        {
            highlightedLine = lineInfoPairs[callingObject];
        }
        else
        {
            return;
        }

        Image imageComponent = callingObject.Find("Logo").GetComponent<Image>();
        GameObject background = callingObject.Find("Background").gameObject;
        background.SetActive(false);

        switch (highlightedLine.line)
        {
            case "pilgrim":
                imageComponent.sprite = pilgrimSprite;
                break;

            case "gallium":
                imageComponent.sprite = galliumSprite;
                break;

            case "pulse":
                imageComponent.sprite = pulseSprite;
                break;
        }
    }

    public void chooseLine(Transform callingObject)
    {
        if (DialogueManager.GetInstance().tutorialStage < 3) return;


        lineInfo chosenLine = new lineInfo();
        if (lineInfoPairs.ContainsKey(callingObject))
        {
            chosenLine = lineInfoPairs[callingObject];
        } else
        {
            return;
        }


        mapManager.currentLine = chosenLine.line;
        mapManager.currentDirection = chosenLine.direction;
        lineInfoPairs.Clear();

        subwayManager.instance.startPlayDoorSFX();

        subwayUI.instance.refreshUI(chosenLine.time, 0.5f);
        nodeManager.instance.updateCurrentStationVisual();

        if (mapManager.currentLine == "pilgrim")
        {
            trainColors.SetTexture("_MainTex", pilgrimColors);
        }
        else if (mapManager.currentLine == "gallium")
        {
            trainColors.SetTexture("_MainTex", galliumColors);
        }
        else if (mapManager.currentLine == "pulse")
        {
            trainColors.SetTexture("_MainTex", pulseColors);
        }
        else
        {
            Debug.LogWarning("a line name was wrong");
        }

        StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToCar));
        if (DialogueManager.GetInstance().tutorialStage == 3)
        {
            DialogueManager.GetInstance().tutorialStage = 4;
        }
        
    }

    public bool checkLastStation()
    {
        mapNode currentNode = mapManager.currentNode;

        switch (mapManager.currentLine)
        {
            case "pulse":
                return currentNode.pulseConnectedNodes[mapManager.currentDirection] == currentNode;
            case "pilgrim":
                return currentNode.pilgrimConnectedNodes[mapManager.currentDirection] == currentNode;
            case "gallium":
                return currentNode.galliumConnectedNodes[mapManager.currentDirection] == currentNode;
        }

        return false;
    }
}
