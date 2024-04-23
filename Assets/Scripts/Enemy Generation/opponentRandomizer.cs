using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opponentRandomizer : MonoBehaviour
{
    [Header("Settings")]
    private int difficultyLevel; //1 to 3
    [SerializeField] int maxDifficulty;
    [SerializeField] float maxVariation;
    [SerializeField] AnimationCurve healthCurve, speedCurve, attackCurve, defenseCurve;
    [SerializeField] int minHealth, maxHealth, minSpeed, maxSpeed, minAttack, maxAttack, minDefense, maxDefense;

    [SerializeField] int debugDifficulty;
    public opponentStats generateStats(int difficulty)
    {
        difficultyLevel = difficulty;
        opponentStats newStats = ScriptableObject.CreateInstance<opponentStats>();

        newStats.difficulty = Random.Range((float)difficulty - maxVariation, (float)difficulty + maxVariation);
        newStats.aggression = newStats.difficulty/((float)maxDifficulty+maxVariation);
        newStats.health = chooseStat("health");
        newStats.attack = chooseStat("attack");
        newStats.defense = chooseStat("defense");

        float speed = Mathf.Lerp(minSpeed, maxSpeed, speedCurve.Evaluate(Random.Range(0f, 1f)));
        newStats.speed = Mathf.RoundToInt(speed);

        return newStats;
    }


    private int chooseStat(string stat)
    {
        AnimationCurve currentCurve = null;
        int currentMin = -1;
        int currentMax = -1;
        float currentDifficulty = difficultyLevel;

        //set stats
        switch (stat)
        {
            case "health":
                currentCurve = healthCurve;
                currentMin = minHealth;
                currentMax = maxHealth;
                break;

            case "speed":
                currentCurve = speedCurve;
                currentMin = minSpeed;
                currentMax = maxSpeed;
                break;

            case "attack":
                currentCurve = attackCurve;
                currentMin = minAttack;
                currentMax = maxAttack;
                break;

            case "defense":
                currentCurve = defenseCurve;
                currentMin = minDefense;
                currentMax = maxDefense;
                break;
        }

        if (currentCurve == null || currentMin == -1 || currentMax == -1)
        {
            Debug.LogWarning("Curve info not found!\nInputed stat was: " + stat);
            return 3;
        }

        //set value
        currentDifficulty = Random.Range(currentDifficulty - maxVariation, currentDifficulty + maxVariation);
        if (currentDifficulty <= 0)
        {
            currentDifficulty = 0;
        } 
        float relativeDifficulty = currentDifficulty / (maxDifficulty+maxVariation); //in percentage
        float curveValue = currentCurve.Evaluate(relativeDifficulty);
        float statValue = Mathf.Lerp(currentMin, currentMax, curveValue);

        return Mathf.RoundToInt(statValue);
    }
}
