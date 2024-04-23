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
    [SerializeField] float passengerExchangePercentage;
    private int totalSpots;

    private void Awake()
    {
        opponentGenerator = GetComponent<opponentRandomizer>();
        totalSpots = sittingSpots.Count;

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
            return;
        }

        int passengerExchangeAmount = (int)((float)npcList.Count * passengerExchangePercentage);

        for (int i = 0; i < passengerExchangeAmount; i++)
        {
            removeRandomOpponent();
            spawnOpponent();
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
        talkableNPC.Remove(removedNPC);
        npcList.Remove(removedNPC);
        Destroy(removedNPC);
    }

    public void spawnOpponent()
    {
        //Find spawn position
        float randomNum = Random.Range(0, 1);
        string spotList = "";
        Transform spotParent = gameObject.transform;
        GameObject newOpponent = null;

        if (randomNum < (float)sittingSpots.Count/(float)totalSpots)
        {
            spotList = "sitting";
        }

        switch (spotList)
        {
            case "sitting":
                spotParent = sittingSpots[sittingSpots.Count - 1];
                break;
        }

        if (spotParent.childCount > 0 || spotParent == gameObject.transform)
        {
            return;
        }

        float randomPrefab = Random.Range(0f, 1f);

        //Instantitate object
        switch(spotList)
        {
            case "sitting":
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

                newTransform.position = spotParent.transform.position;
                if (newTransform.position.x > 0)
                {
                    newTransform.eulerAngles = new Vector3(0, 180, 0);
                }

                sittingSpots.Insert(0, spotParent);
                sittingSpots.RemoveAt(sittingSpots.Count - 1);
                break;
        }

        if (newOpponent == null)
        {
            Debug.LogWarning("No new opponent generated!");
            return;
        }

        int difficulty = generateDifficulty();
        newOpponent.GetComponent<opponentInfo>().stats = opponentGenerator.generateStats(difficulty);
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

        print("current stage is " + currentStage + ", stageProgress is " + stageProgress);

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
}
