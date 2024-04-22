using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class combatManager : MonoBehaviour
{
    public static combatManager instance { get; private set; }
    [HideInInspector] public opponentStats opponent;
    [HideInInspector] public string state; //player-choose, player-effects, opponent-choose, opponent-effects, increment speed
    public List<card> drawPile;
    public List<card> discardPile;
    [HideInInspector] public List<card> playerHand;


    [Header("Setup")]
    [SerializeField] npcManager npcManagerScript;
    [SerializeField] cardOptions victoryCardSpawner;
    [SerializeField] GameObject combatParent;
    [SerializeField] GameObject victoryParent;

    [Header("CombatSettings")]
    public float speedIncrementSpeed;
    [SerializeField] float cardPlayedHoverDuration; //for card
    

    [Header("EnemySettings")]
    [SerializeField] int opponentDeckSize;

    [Header("MiscSettings")]
    [SerializeField] int cardTimeMultiplier;

    private int playerSpeed, playerMaxHealth, playerHealth;
    private int opponentSpeed, opponentMaxHealth, opponentHealth;
    private int tempPlayerHealth, tempOpponentHealth; //for retort

    private int playerSpeedCounter, opponentSpeedCounter; 
    private string lastIncremented; //for keeping track of which counter to increment after a participant takes a turn

    private combatUI uiScript;
    private cardEffect effectsScript;

    private List<card> opponentCards = new List<card>();

    private int cardsPlayed; //for time

    [HideInInspector] public bool isPlayerCursed, isOpponentCursed;
    [HideInInspector] public bool endPlayerTurnCursed, endOpponentTurnCursed;
    [SerializeField] card cursedCard;
    [HideInInspector] public bool isPlayerToRedraw, isOpponentToRedraw;
    [HideInInspector] public bool isPlayerMultiPlay, isOpponentMultiPlay;

    //Card effect Variables
    [HideInInspector] public int bonusPlayerAttack, bonusOpponentAttack;
    [HideInInspector] public int bonusPlayerDefend, bonusOpponentDefend;
    public int playerHandAmount;


    private void Awake()
    {
        instance = this;
        uiScript = GetComponent<combatUI>();
        effectsScript = GetComponent<cardEffect>();
        uiScript.setDefaultPositions();
    }

    public void startCombat()
    {
        //isPlayerCursed = true;
        combatParent.SetActive(true);
        victoryParent.SetActive(false);

        setStats();
        DialogueManager.GetInstance().ClearAll();
        TextMovement.GetInstance().ResetPos();
        DialogueManager.GetInstance().EnterNarration();

        uiScript.resetUIs();

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
    }

    public void fight()
    {
        StartCoroutine(incrementSpeed());
    }

    private void setStats()
    {
        playerSpeed = gameManager.instance.playerSpeed;
        opponentSpeed = opponent.speed;

        playerMaxHealth = gameManager.instance.playerMaxHealth;
        opponentMaxHealth = opponent.health;

        playerHealth = gameManager.instance.playerHealth;
        opponentHealth = opponentMaxHealth;

        tempPlayerHealth = playerHealth;
        tempOpponentHealth = opponentHealth;

        playerSpeedCounter = 0;
        opponentSpeedCounter = 0;
        cardsPlayed = 0;

        bonusPlayerAttack = 0;
        bonusOpponentAttack = 0;
        bonusPlayerDefend = 0;
        bonusOpponentDefend = 0;
        playerHandAmount = 3;
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

        for (int i = 0; i < playerHandAmount; i++)
        {
            drawCard();
        }
    }

    private void startOpponentTurn()
    {
        state = "opponent-choose";

        StartCoroutine(playOpponentCard());
    }

    IEnumerator playOpponentCard()
    {
        yield return new WaitForSeconds(cardPlayedHoverDuration*1.5f);
        print(opponentCards.Count);
        card currentCard = opponentCards[Random.Range(0, opponentCards.Count)]; // CHANGE AFTER ADDING RETORT
        if (isOpponentCursed)
        {
            currentCard = cursedCard;
        }

        DialogueManager.GetInstance().EnterCombat("Opponent", currentCard.type.ToString());

        
        switch (currentCard.type)
        {
            case card.cardType.Attack:
                inflictDamage(0, currentCard.cardStrength);
                break;

            case card.cardType.Defend:
                retort(1, currentCard.cardStrength);
                break;

            case card.cardType.Effect:
                break;
        }

        if (!isOpponentToRedraw)
        {
            opponentHealth = tempOpponentHealth;
        }
        
        uiScript.updateHealthUI(opponentHealth, opponentMaxHealth, tempOpponentHealth, playerHealth, playerMaxHealth, tempPlayerHealth);

        yield return new WaitForSeconds(cardPlayedHoverDuration/2);

        cardsPlayed++;

        if (isOpponentMultiPlay)
        {
            isOpponentMultiPlay = false;
            yield break;
        }

        if (isOpponentToRedraw)
        {
            isOpponentToRedraw = false;
            startOpponentTurn();
            yield break;
        }

        if (!checkCombatEnd())
        {
            endOpponentRound();
        }
        else
        {
            while (true)
            {
                if (DialogueManager.GetInstance().canContinueToNextLine == true)
                {
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            yield return null;
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
        if (isPlayerCursed == false)
        {
            playerHand.Add(drawPile[0]);
            uiScript.spawnCard(drawPile[0], false, Vector3.zero);
            drawPile.RemoveAt(0);
        } else if (isPlayerCursed == true)
        {
            uiScript.spawnCard(cursedCard, false, Vector3.zero);
        }

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
        isPlayerCursed = endPlayerTurnCursed;
        endPlayerTurnCursed = false;
        isPlayerMultiPlay = false;
        uiScript.discardAllCards();
        StartCoroutine(incrementSpeed());

    }

    private void endOpponentRound()
    {
        isOpponentCursed = endOpponentTurnCursed;
        endOpponentTurnCursed = false;
        isOpponentMultiPlay = false;
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
                retort(0, cardInfo.cardStrength);
                break;

            case card.cardType.Effect:
                effectsScript.playCard(cardInfo, 0);
                break;
        }

        if (!isPlayerToRedraw && !isPlayerMultiPlay)
        {
            playerHealth = tempPlayerHealth;
        }
        
        uiScript.updateHealthUI(opponentHealth, opponentMaxHealth, tempOpponentHealth, playerHealth, playerMaxHealth, tempPlayerHealth);

        yield return new WaitForSeconds(cardPlayedHoverDuration);

        uiScript.discardCard(playedCard);

        yield return new WaitForSeconds(cardPlayedHoverDuration/4);

        cardsPlayed++;

        if (isPlayerMultiPlay && playerHand.Count > 0)
        {
            yield return new WaitForSeconds(0.2f);
            uiScript.hasSelectedCard = false;
            isPlayerMultiPlay = false;
            yield break;
        }

        if (isPlayerToRedraw)
        {
            isPlayerToRedraw = false;
            uiScript.discardAllCards();
            yield return new WaitForSeconds(0.5f);
            startPlayerTurn();
            yield break;
        }
        if (!checkCombatEnd())
        {
            endPlayerRound();
        } else
        {
            while (true)
            {
                if (DialogueManager.GetInstance().canContinueToNextLine == true)
                {
                    break;
                } else
                {
                    yield return null;
                }
            }

            yield return null;

            endCombat();
        }
    }

    public void inflictDamage(int target, int amount)
    {
        if (target == 0) //targeting player
        {
            tempPlayerHealth -= amount + bonusOpponentAttack;
            if (tempPlayerHealth <= 0)
            {
                tempPlayerHealth = 0;
                playerHealth = 0;
            }
        } else if (target == 1) //targeting opponent
        {
            tempOpponentHealth -= amount + bonusPlayerAttack;
            if (tempOpponentHealth <= 0)
            {
                tempOpponentHealth = 0;
                opponentHealth = 0;
            }
        }
    }

    public void inflictSimpleDamage(int target, int amount) //does not take into account bonusDamage
    {
        if (target == 0) //targeting player
        {
            tempPlayerHealth -= amount;
            if (tempPlayerHealth <= 0)
            {
                tempPlayerHealth = 0;
                playerHealth = 0;
            }
        }
        else if (target == 1) //targeting opponent
        {
            tempOpponentHealth -= amount;
            if (tempOpponentHealth <= 0)
            {
                tempOpponentHealth = 0;
                opponentHealth = 0;
            }
        }
    }

    public void retort(int target, int amount)
    {
        if (target == 0)
        {
            tempPlayerHealth += amount + bonusPlayerDefend;
            if (tempPlayerHealth > playerHealth)
            {
                tempPlayerHealth = playerHealth;
            }
        } else if (target == 1)
        {
            tempOpponentHealth += amount + bonusOpponentDefend;
            if (tempOpponentHealth > opponentHealth)
            {
                tempOpponentHealth = opponentHealth;
            }
        }
    }
    public void simpleRetort(int target, int amount)    //does not take into account bonusDefend
    {
        if (target == 0)
        {
            tempPlayerHealth += amount;
            if (tempPlayerHealth > playerHealth)
            {
                tempPlayerHealth = playerHealth;
            }
        }
        else if (target == 1)
        {
            tempOpponentHealth += amount;
            if (tempOpponentHealth > opponentHealth)
            {
                tempOpponentHealth = opponentHealth;
            }
        }
    }


    public void heal(int target, int amount)
    {
        if (target == 0)
        {
            playerHealth += amount;

            if (playerHealth > playerMaxHealth)
            {
                playerHealth = playerMaxHealth;
            }
            tempPlayerHealth = playerHealth;
        } else if (target == 1)
        {
            opponentHealth += amount;

            if (opponentHealth > opponentMaxHealth)
            {
                opponentHealth = opponentMaxHealth;
            }
            tempOpponentHealth = opponentHealth;
        }
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
        npcManagerScript.updateCar();

        uiScript.clearCards();
        gameManager.instance.playerHealth = playerHealth;
        combatParent.SetActive(true);

        combatParent.SetActive(false);

        uiScript.updateHealthUI(1, 1, 1, playerHealth, playerMaxHealth, tempPlayerHealth);
        if (state == "loss")
        {
            npcManagerScript.removeAll();
            subwayUI.instance.refreshUI(cardsPlayed * cardTimeMultiplier, 2);
            subwayManager.instance.switchToMovement();
        }

        if (state == "victory")
        { 
            gameManager.instance.followerAmount++;
            subwayUI.instance.refreshUI(cardsPlayed * cardTimeMultiplier, 2);

            victoryParent.SetActive(true);
            victoryCardSpawner.spawnNewCards(opponent.aggression);
        }
    }

    private void createOpponentDeck()
    {
        int effectAmount = Random.Range(0, 3);
        float balancedAggression = opponent.aggression - 0.1f;
        if (balancedAggression >= 0.7)
        {
            balancedAggression = 0.7f;
        } else if (balancedAggression <= 0.4)
        {
            balancedAggression = 0.4f;
        }
        int attackAmount = Mathf.RoundToInt(opponentDeckSize* balancedAggression);

        int addedEffectAmount = 0;
        int addedAttackAmount = 0;

        for (int i = 0; i < opponentDeckSize; i++)
        {
            card newCard = ScriptableObject.CreateInstance<card>();
            
            if (addedEffectAmount < effectAmount) //effect
            {
                newCard.type = card.cardType.Effect;
                addedEffectAmount++;

            } else if (addedAttackAmount < attackAmount) //attack
            {
                newCard.type = card.cardType.Attack;
                newCard.cardStrength = Random.Range(opponent.attack - 2, opponent.attack + 3);
                addedAttackAmount++;
            } else //defense
            {
                newCard.type = card.cardType.Defend;
                newCard.cardStrength = Random.Range(opponent.defense - 2, opponent.defense + 3);
            }

            opponentCards.Add(newCard);
        }
    }

    public void decreaseSpeed(int target)
    {
        if (target == 0)
        {
            playerSpeed++;
            
        } else if (target == 1)
        {
            opponentSpeed++;
        }

        uiScript.updateSpeedUI((float)opponentSpeedCounter / (float)opponentSpeed, (float)playerSpeedCounter / (float)playerSpeed);
    }
}
