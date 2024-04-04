using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviour
{
    public static combatManager instance { get; private set; }

    [Header("Public Access")]
    public string state; //player-choose, player-effects, opponent-choose, opponent-effects, increment speed, player-retort, opponent-retort
    public List<card> drawPile;
    public List<card> discardPile;
    public List<card> playerHand;

    [Header("Setup")]
    [SerializeField] npcManager npcManagerScript;

    [Header("Settings")]
    public float speedIncrementSpeed;
    [SerializeField] float cardPlayedHoverDuration; //for card
    public int playerHandAmount;
    
    private int playerSpeed, playerAttack, playerMaxHealth, playerHealth, playerDefense;
    private int opponentSpeed, opponentAttack, opponentMaxHealth, opponentHealth, opponentDefense;

    private int playerSpeedCounter, opponentSpeedCounter;
    private string lastIncremented; //for keeping track of which counter to increment after a participant takes a turn
    private string lastPlayed; //for keeping track of whos turn it was last

    private opponentRandomizer opponentStats;
    private combatUI uiScript;
    private cardEffect effectsScript;

    private string[] opponentActions = new string[] { "Attack", "Effect", "Defend" };

    private void Awake()
    {
        instance = this;
        opponentStats = GetComponent<opponentRandomizer>();
        uiScript = GetComponent<combatUI>();
        effectsScript = GetComponent<cardEffect>();
        uiScript.setDefaultPositions();
    }


    public void Start()
    {
        //setup

    }

    public void startCombat()
    {
        setStats();
        DialogueManager.GetInstance().ClearAll();
        TextMovement.GetInstance().ResetPos();
        DialogueManager.GetInstance().EnterNarration();

        uiScript.updateHealthUI((float)opponentHealth / (float)opponentMaxHealth, (float)playerHealth / (float)playerMaxHealth);
        uiScript.updateSpeedUI((float)opponentSpeedCounter/(float)opponentSpeed, (float)playerSpeedCounter/(float)playerSpeed);
        uiScript.updateDefenseUI(opponentDefense, playerDefense);

        discardPile.Clear();
        drawPile.Clear();
        playerHand.Clear();

        for (int i = 0; i  < gameManager.instance.playerDeck.Count; i++)
        {
            drawPile.Add(gameManager.instance.playerDeck[i]);
        }

        shuffleDrawPile();

        lastIncremented = "opponent"; //starts on player turn

        //StartCoroutine(incrementSpeed());
    }

    public void fight()
    {
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

        playerSpeedCounter = 0;
        opponentSpeedCounter = 0;
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

            uiScript.updateSpeedUI((float)opponentSpeedCounter / (float)opponentSpeed, (float)playerSpeedCounter / (float)playerSpeed);

            //has reached turn check
            bool hasReachedTurn = false;
            if (opponentSpeedCounter == opponentSpeed)
            {
                yield return new WaitForSeconds(speedIncrementSpeed/2);
                opponentSpeedCounter = 0;
                startOpponentTurn();
                hasReachedTurn = true;

            } else if (playerSpeedCounter == playerSpeed)
            {
                yield return new WaitForSeconds(speedIncrementSpeed / 2);
                playerSpeedCounter = 0;
                startPlayerTurn();
                hasReachedTurn = true;
            }

            
            if (hasReachedTurn) break;
            yield return new WaitForSeconds(speedIncrementSpeed);
        }
    }

    private void startPlayerTurn()
    {
        state = "player-choose";
        uiScript.hasSelectedCard = false;

        if (lastPlayed == "opponent")
        {
            playerDefense = 0;
            uiScript.updateDefenseUI(opponentDefense, playerDefense);
        }

        for (int i = 0; i < playerHandAmount; i++)
        {
            drawCard();
        }
        lastPlayed = "player";
    }

    private void startOpponentTurn()
    {
        state = "opponent-choose";

        if (lastPlayed == "player")
        {
            opponentDefense = 0;
            uiScript.updateDefenseUI(opponentDefense, playerDefense);
        }

        DialogueManager.GetInstance().EnterCombat("Opponent", opponentActions[Random.Range(0, opponentActions.Length)]);

        inflictDamage(0, 10);

        lastPlayed = "opponent";

        endOpponentRound();
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
            print("shuffle discard to draw pile");

            //move discardPile to drawPile
            while (discardPile.Count > 0)
            {
                drawPile.Add(discardPile[0]);
                discardPile.RemoveAt(0);
            }
            shuffleDrawPile();
        }
    }


    private void endPlayerRound()
    {
        uiScript.discardAllCards();
        StartCoroutine(incrementSpeed());

    }

    private void endOpponentRound()
    {
        StartCoroutine(incrementSpeed());
    }

    public void startPlayCard(GameObject playedCard, card cardInfo)
    {
        StartCoroutine(playCard(playedCard, cardInfo));
    }

    IEnumerator playCard(GameObject playedCard, card cardInfo)
    {
        DialogueManager.GetInstance().EnterCombat("Player", cardInfo.type.ToString());
        //ACTIVE CARD EFFECT HERE
        switch (cardInfo.type)
        {
            case card.cardType.Attack:
                inflictDamage(1, cardInfo.cardStrength);
                break;

            case card.cardType.Defend:
                increaseDefense(0, cardInfo.cardStrength);
                break;

            case card.cardType.Effect:
                break;
        }

        yield return new WaitForSeconds(cardPlayedHoverDuration);

        uiScript.discardCard(playedCard);

        yield return new WaitForSeconds(cardPlayedHoverDuration/4);

        if (!checkCombatEnd())
        {
            endPlayerRound();
        } else
        {
            endCombat();
        }
        
    }

    public void inflictDamage(int target, int amount)
    {
        if (target == 0) //targeting player
        {
            playerDefense -= opponentAttack + amount;

            if (playerDefense < 0)
            {
                playerHealth -= Mathf.Abs(playerDefense);
                playerDefense = 0;
            }
        } else if (target == 1) //targeting opponent
        {
            opponentDefense -= playerAttack + amount;

            if (opponentDefense < 0)
            {
                opponentHealth -= Mathf.Abs(opponentDefense);
                opponentDefense = 0;
            }
        }

        uiScript.updateHealthUI((float)opponentHealth / (float)opponentMaxHealth, (float)playerHealth / (float)playerMaxHealth);
        uiScript.updateDefenseUI(opponentDefense, playerDefense);

        print("opponent health: " + opponentHealth + "/" + (float)opponentMaxHealth); // for debugging
        print("player health: " + playerHealth + "/" + (float)playerMaxHealth);
    }

    public void increaseDefense(int target, int amount)
    {
        if (target == 0)
        {
            playerDefense += amount;
        } else if (target == 1)
        {
            opponentDefense += amount;
        }

        uiScript.updateDefenseUI(opponentDefense, playerDefense);
    }

    private bool checkCombatEnd()
    {
        if (playerHealth <= 0)
        {
            state = "loss";
            playerHealth = 0;
            return true;
        } else if (opponentHealth <= 0)
        {
            state = "victory";
            return true;
        }

        return false;
    }

    private void endCombat()
    {

        nodeManager.instance.progressStation();

        uiScript.clearCards();
        gameManager.instance.playerHealth = playerHealth;
        subwayManager.instance.switchToMovement();

        if (state == "loss")
        {
            npcManagerScript.removeAll();
        }

        if (state == "victory")
        {
            //get to choose new card
        }
    }
}
