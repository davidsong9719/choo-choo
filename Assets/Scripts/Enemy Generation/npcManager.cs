using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcManager : MonoBehaviour
{
    public List<GameObject> npcList; //hide in insecter after adding npc generator script and change to private set
    public List<GameObject> talkableNPC;

    [SerializeField] List<Transform> standingSpotsL;
    [SerializeField] List<Transform> standingSpotsR;

    public List<Transform> sittingSpots;

    [SerializeField] GameObject sittingPrefab;
    private opponentRandomizer opponentGenerator;

    [Header("Difficulty Distribution")]
    [SerializeField] AnimationCurve stageOneStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageOneEndingDifficultyDistribution;

    [SerializeField] AnimationCurve stageTwoStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageTwoEndinggDifficultyDistribution;

    [SerializeField] AnimationCurve stageThreeStartingDifficultyDistribution;
    [SerializeField] AnimationCurve stageThreeEndingDifficultyDistribution;

    private void Awake()
    {
        opponentGenerator = GetComponent<opponentRandomizer>();
    }

    private void Start()
    {
        addOpponents();
    }
    public void removeFromList(GameObject npc)
    {
        talkableNPC.Remove(npc);
    }

    public void removeAll()
    {
        talkableNPC.Clear();
    }

    public void addOpponents()
    {
        //***** ACTUALLY ADD THE FUCKING physical GENERATION 

        foreach (GameObject npc in npcList)
        {
            int difficulty = generateDifficulty();
            npc.GetComponent<opponentInfo>().stats = opponentGenerator.generateStats(difficulty);
        }
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

        int difficulty = (int)(difficultyFloat * 2);
        return difficulty;
    } 
}
