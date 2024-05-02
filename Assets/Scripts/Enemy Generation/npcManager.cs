using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class npcManager : MonoBehaviour
{
    public List<GameObject> npcList; //hide in insecter after adding npc generator script and change to private set
    public List<GameObject> talkableNPC;

    [SerializeField] List<Transform> sittingSpots;
    private List<Transform> openSpots = new List<Transform>();
    [SerializeField] stationManager stationManagerScript;

    [SerializeField] GameObject sittingPrefab0, sittingPrefab1, sittingPrefab2;
    private opponentRandomizer opponentGenerator;

    [Header("Difficulty Distribution")]
    [SerializeField] AnimationCurve stageOneStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageOneEndingDifficultyDistribution;

    [SerializeField] AnimationCurve stageTwoStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageTwoEndinggDifficultyDistribution;

    [SerializeField] AnimationCurve stageThreeStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageThreeEndingDifficultyDistribution;

    [Header("Density Settings")]
    [SerializeField] AnimationCurve passengerDensityCurve;
    [SerializeField] int maxVariation;
    private int totalSpots;

    [Header("Visual Settings")]
    [SerializeField] Material stageOneMaterial, stageTwoMaterial, stageThreeMaterial;

    private void Awake()
    {
        opponentGenerator = GetComponent<opponentRandomizer>();
        totalSpots = sittingSpots.Count;
        openSpots = copyList(sittingSpots);

        sittingSpots = shuffleTransforms(sittingSpots);
    }

    private void Start()
    {
        updateCar();
    }
    public void removeFromList(GameObject npc)
    {
        talkableNPC.Remove(npc);
    }

    public void removeAll()
    {
        talkableNPC.Clear();
    }

    public void updateCar()
    {
        if (stationManagerScript.checkLastStation())
        {
            for (int i = 0; i < npcList.Count; i++)
            {
                Destroy(npcList[i]);
            }

            npcList.Clear();
            talkableNPC.Clear();
            openSpots = copyList(sittingSpots);
            return;
        }

        int passengerExchangeAmount = npcList.Count;

        while (npcList.Count > 0)
        {
            GameObject removedNPC = npcList[0];

            openSpots.Add(removedNPC.GetComponent<opponentInfo>().transformParent);
            npcList.Remove(removedNPC);

            if (talkableNPC.Contains(removedNPC))
            {
                talkableNPC.Remove(removedNPC);
            }

            Destroy(removedNPC);
        }

        float stageProgress = 0;

        stageProgress = (float)gameManager.instance.timeElapsed / (float)(gameManager.instance.stageOneLength + gameManager.instance.stageTwoLength + gameManager.instance.stageThreeLength);

        float passengerDensity = passengerDensityCurve.Evaluate(stageProgress);

        int passengerAmount = (int)Mathf.Lerp(6 + maxVariation, totalSpots-maxVariation, passengerDensity);

        passengerAmount = Random.Range(passengerAmount - maxVariation, passengerAmount + maxVariation);

        int passengerDifference = passengerAmount - npcList.Count;

        if (passengerDifference > 0)
        {
            for (int i =0; i < Mathf.Abs(passengerDifference); i++)
            {
                spawnOpponent();
            }
        } else
        {
            for (int i = 0; i < Mathf.Abs(passengerDifference); i++)
            {
                removeRandomOpponent();
            }
        }
    }

    private void removeRandomOpponent()
    {
        GameObject removedNPC = npcList[Random.Range(0, npcList.Count)];

        openSpots.Add(removedNPC.GetComponent<opponentInfo>().transformParent);
        npcList.Remove(removedNPC);
        
        if(talkableNPC.Contains(removedNPC))
        {
            talkableNPC.Remove(removedNPC);
        }

        Destroy(removedNPC);
    }

    public void spawnOpponent()
    {
        //Find spawn position
        float randomNum = Random.Range(0, 1);

        Transform spotParent = transform;
        if (openSpots.Count > 0)
        {
            spotParent = openSpots[Random.Range(0, openSpots.Count)];
        } else
        {
            Debug.LogWarning("No open seats left!");
            return;
        }

        GameObject newOpponent = null;
        float randomPrefab = Random.Range(0f, 1f);

        //Instantitate object
        if (randomPrefab < 0.33)
        {
            newOpponent = Instantiate(sittingPrefab0);
        } else if (randomPrefab < 0.66)
        {
            newOpponent = Instantiate(sittingPrefab1);
        } else
        {
            newOpponent = Instantiate(sittingPrefab2);
        }
        
        Transform newTransform = newOpponent.transform;


        newTransform.position = spotParent.position;
        openSpots.Remove(spotParent);
        opponentInfo opponentScript = newOpponent.GetComponent<opponentInfo>();
        opponentScript.transformParent = spotParent;

        if (newTransform.position.x > 0)
        {
            newTransform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Random.Range(0f, 1f) > 0.5)
        {
            npcList.Add(newOpponent);
            return;
        }

        int difficulty = generateDifficulty();
        
        opponentScript.stats = opponentGenerator.generateStats(difficulty);
        
        switch(difficulty)
        {
            case 0:
                newOpponent.GetComponentInChildren<Renderer>().material = stageOneMaterial;
                break;

            case 1:
                newOpponent.GetComponentInChildren<Renderer>().material = stageTwoMaterial;
                break;

            case 2:
                newOpponent.GetComponentInChildren<Renderer>().material = stageThreeMaterial;
                break;
        }
        npcList.Add(newOpponent);
        talkableNPC.Add(newOpponent);
    }

    private int generateDifficulty()
    {
        float animationCurvePosition = Random.Range(0, 1);

        float stageProgress = 0;
        string currentStage = "";

        if (gameManager.instance.timeElapsed < gameManager.instance.stageOneLength)
        {
            currentStage = "1";
            stageProgress = (float)gameManager.instance.timeElapsed/(float)gameManager.instance.stageOneLength;
        } else if (gameManager.instance.timeElapsed - gameManager.instance.stageOneLength < gameManager.instance.stageTwoLength)
        {
            currentStage = "2";
            stageProgress = ((float)gameManager.instance.timeElapsed - (float)gameManager.instance.stageOneLength) / (float)gameManager.instance.stageTwoLength;
        } else
        {
            currentStage = "3";
            stageProgress = (((float)gameManager.instance.timeElapsed - (float)gameManager.instance.stageOneLength) - (float)gameManager.instance.stageTwoLength) / (float) gameManager.instance.stageThreeLength;
        }

        float difficultyFloat = 0;
        float randomFloat = Random.Range(0, 1);

        switch (currentStage)
        {
            case "1":             
                if (randomFloat > stageProgress)
                {
                    difficultyFloat = stageOneStartingDifficultyDistribution.Evaluate(animationCurvePosition);
                } else
                {
                    difficultyFloat = stageOneEndingDifficultyDistribution.Evaluate(animationCurvePosition);
                }
                break;

            case "2":
                if (randomFloat > stageProgress)
                {
                    difficultyFloat = stageTwoStartingDifficultyDistribution.Evaluate(animationCurvePosition);
                }
                else
                {
                    difficultyFloat = stageTwoEndinggDifficultyDistribution.Evaluate(animationCurvePosition);
                }
                break;

            case "3":
                if (randomFloat > stageProgress)
                {
                    difficultyFloat = stageThreeStartingDifficultyDistribution.Evaluate(animationCurvePosition);
                }
                else
                {
                    difficultyFloat = stageThreeEndingDifficultyDistribution.Evaluate(animationCurvePosition);
                }
                break;

            default:
                Debug.LogError("Stage not found. Inputed stage was: " + currentStage);
                break;
        }

        int difficulty = (int)difficultyFloat-1; //the -1 is there to counteract changes to the animation curve, don't touch no matter what lol
        return difficulty;
    } 

    private List<Transform> shuffleTransforms(List<Transform> oldList)
    {
        List<Transform> newList = new List<Transform>();

        while(oldList.Count > 0)
        {
            int currentIndex = Random.Range(0, oldList.Count);
            newList.Add(oldList[currentIndex]);
            oldList.RemoveAt(currentIndex);
        }


        return newList;
    }

    private List<Transform> copyList(List<Transform> oldList)
    {
        List<Transform> newList = new List<Transform> ();
        for (int i = 0; i < oldList.Count; i++)
        {
            newList.Add(oldList[i]);
        }

        return newList;
    }
}
