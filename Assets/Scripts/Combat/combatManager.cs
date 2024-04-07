using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviour
{
    public static combatManager instance { get; private set; }
    [HideInInspector] public opponentStats opponent;
    [HideInInspector] public string state; //player-choose, player-effects, opponent-choose, opponent-effects, increment speed, player-retort, opponent-retort
    [HideInInspector] public List<card> drawPile;
    [HideInInspector] public List<card> discardPile;
    [HideInInspector] public List<card> playerHand;


    [Header("Setup")]
    [SerializeField] npcManager npcManagerScript;

    [Header("CombatSettings")]
    public float speedIncrementSpeed;
    [SerializeField] float cardPlayedHoverDuration; //for card
    public int playerHandAmount;
    [SerializeField] int victoryCardAmount;

    [Header("EnemySettings")]
    [SerializeField] int opponentDeckSize;
    
    private int playerSpeed, playerAttack, playerMaxHealth, playerHealth, playerDefense;
    private int opponentSpeed, opponentAttack, opponentMaxHealth, opponentHealth, opponentDefense;

    private int playerSpeedCounter, opponentSpeedCounter; 
    private string lastIncremented; //for keeping track of which counter to increment after a participant takes a turn
    private string lastPlayed; //for keeping track of whos turn it was last

    private combatUI uiScript;
    private cardEffect effectsScript;

    private List<card> opponentCards = new List<card>();

    private void Awake()
    {
        instance = this;
        uiScript = GetComponent<combatUI>();
        effectsScript = GetComponent<cardEffect>();
        uiScript.setDefaultPositions();
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
        opponentCards.Clear();
        createOpponentDeck();

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
        playerSpeed = gameManager.instance.playerSpeed;
        opponentSpeed = opponent.speed;

        playerAttack = gameManager.instance.playerAttack;
        opponentAttack = opponent.attack;

        playerMaxHealth = gameManager.instance.playerMaxHealth;
        opponentMaxHealth = opponent.health;

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

        StartCoroutine(playOpponentCard());

        lastPlayed = "opponent";


    }

    IEnumerator playOpponentCard()
    {
        yield return new WaitForSeconds(cardPlayedHoverDuration*1.5f);
        print(opponentCards.Count);
        card currentCard = opponentCards[Random.Range(0, opponentCards.Count)]; // CHANGE AFTER ADDING RETORT
        
        DialogueManager.GetInstance().EnterCombat("Opponent", currentCard.type.ToString());

        switch (currentCard.type)
        {
            case card.cardType.Attack:
                inflictDamage(0, currentCard.cardStrength);
                break;

            case card.cardType.Defend:
                increaseDefense(1, currentCard.cardStrength);
                break;

            case card.cardType.Effect:
                break;
        }

        yield return new WaitForSeconds(cardPlayedHoverDuration/2);

        if (!checkCombatEnd())
        {
            endOpponentRound();
        }
        else
        {
            endCombat();
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
            for (int i = 0; i < victoryCardAmount; i++)
            {
                cardGenerator.instance.generateNewCard(opponent.aggression);
            }
            //get to choose new card
        }
    }

    private void createOpponentDeck()
    {
        int effectAmount = Random.Range(0, 3);
        float balancedAggression = opponent.aggression - 0.1f;
        if (balancedAggression >= 0.6)
        {
            balancedAggression = 0.6f;
        } else if (balancedAggression <= 0.2)
        {
            balancedAggression = 0.2f;
        }
        int attackAmount = Mathf.RoundToInt((opponentDeckSize-effectAmount)* balancedAggression);

        int addedEffectAmount = 0;
        int addedAttackAmount = 0;

        for (int i = 0; i < opponentDeckSize; i++)
        {
            card newCard = ScriptableObject.CreateInstance<card>();
            
            if (addedEffectAmount < effectAmount) //effect
            {
                newCard.type = card.cardType.Effect;

            } else if (addedAttackAmount < attackAmount) //attack
            {
                newCard.type = card.cardType.Attack;
                newCard.cardStrength = Random.Range(opponent.attack - 2, opponent.attack + 3);
            } else //defense
            {
                newCard.type = card.cardType.Defend;
                newCard.cardStrength = Random.Range(opponent.defense - 2, opponent.defense + 3);
            }

            opponentCards.Add(newCard);
        }
    }
}
