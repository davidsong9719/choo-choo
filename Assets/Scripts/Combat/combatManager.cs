using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class combatManager : MonoBehaviour
{
    public static combatManager instance { get; private set; }

    [Header("Public Access")]
    public string state; //player-choose, player-effects, opponent-choose, opponent-effects, increment speed
    public List<card> drawPile;
    public List<card> discardPile;
    public List<card> playerHand;

    [Header("Settings")]
    [SerializeField] float speedIncrementSpeed;
    [SerializeField] float cardPlayedHoverDuration; //for card
    public int playerHandAmount;
     

    private int playerSpeed, playerAttack, playerMaxHealth, playerHealth;
    private int opponentSpeed, opponentAttack, opponentMaxHealth, opponentHealth;

    private int playerSpeedCounter, opponentSpeedCounter;
    private string lastIncremented; //for keeping track of which counter to increment after a participant takes a turn

    private opponentRandomizer opponentStats;
    private combatUI uiScript;
    private cardEffect effectsScript;
     

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //setup
        opponentStats = GetComponent<opponentRandomizer>();
        uiScript = GetComponent<combatUI>();
        effectsScript = GetComponent<cardEffect>();

        setStats();
        uiScript.setDefaultPositions();

        drawPile = gameManager.instance.playerDeck;
        shuffleDrawPile();

        lastIncremented = "opponent"; //starts on player turn

        StartCoroutine(incrementSpeed());
    }

    private void setStats()
    {
        opponentStats.generateStats();

        playerSpeed = gameManager.instance.playerSpeed;
        opponentSpeed = opponentStats.opponentSpeed;

        playerAttack = gameManager.instance.playerAttack;
        opponentSpeed = opponentStats.opponentSpeed;

        playerMaxHealth = gameManager.instance.playerMaxHealth;
        opponentMaxHealth = opponentStats.opponentMaxHealth;

        playerHealth = gameManager.instance.playerHealth;
        opponentHealth = opponentMaxHealth;
    }

    IEnumerator incrementSpeed()
    {
        state = "increment speed";
        yield return new WaitForSeconds(speedIncrementSpeed);

        while (true)
        {


            //increment
            if (lastIncremented == "player")
            {
                opponentSpeedCounter++;
                lastIncremented = "opponent";

            } else if (lastIncremented == "opponent")
            {
                playerSpeedCounter++;
                lastIncremented = "player";

            }

            //has reached turn check
            bool hasReachedTurn = false;
            if (opponentSpeedCounter == opponentSpeed)
            {
                opponentSpeedCounter = 0;
                state = "opponent-choose";
                hasReachedTurn = true;

            } else if (playerSpeedCounter == playerSpeed)
            {
                playerSpeedCounter = 0;
                startPlayerTurn();
                hasReachedTurn = true;
            }

            uiScript.updateSpeedUI(opponentSpeedCounter, playerSpeedCounter);
            if (hasReachedTurn) break;
            yield return new WaitForSeconds(speedIncrementSpeed);
        }
    }

    private void startPlayerTurn()
    {
        state = "player-choose";
        uiScript.hasSelectedCard = false;

        for (int i = 0; i < playerHandAmount; i++)
        {
            drawCard();
        }

    }

    private void shuffleDrawPile()
    {
        List<card> shuffledPile = new List<card>();

        while (drawPile.Count > 0)
        {
            int randomIndex = Random.Range(0, drawPile.Count);
            shuffledPile.Add(drawPile[randomIndex]);
            drawPile.RemoveAt(randomIndex);
        }



        drawPile = shuffledPile;
    }

    private void drawCard()
    {
        playerHand.Add(drawPile[0]);
        uiScript.spawnCard(drawPile[0], false, Vector3.zero);

        drawPile.RemoveAt(0);

        if (drawPile.Count == 0)
        {
            drawPile = discardPile;
            discardPile.Clear();
            shuffleDrawPile();
        }
    }

    private void endPlayerRound()
    {
        uiScript.discardAllCards();
        StartCoroutine(incrementSpeed());
    }


    IEnumerator playCard(GameObject playedCard, card cardInfo)
    {
        //ACTIVE CARD EFFECT HERE
        effectsScript.playCard(cardInfo, 1);

        yield return new WaitForSeconds(cardPlayedHoverDuration);

        uiScript.discardCard(playedCard);

        yield return new WaitForSeconds(cardPlayedHoverDuration/4);
        endPlayerRound();
    }

    public void startPlayCard(GameObject playedCard, card cardInfo)
    {
        StartCoroutine(playCard(playedCard, cardInfo));
    }
}
