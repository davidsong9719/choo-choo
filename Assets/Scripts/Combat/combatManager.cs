using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
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
    [HideInInspector] public cardOptions victoryCardSpawner;
    [SerializeField] GameObject combatParent;
    [HideInInspector] public GameObject victoryParent;

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

    public List<card> opponentCards = new List<card>();

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

    private bool isTutorial = false;
    private int tutorialCardCycle;

    private void Awake()
    {
        instance = this;
        uiScript = GetComponent<combatUI>();
        effectsScript = GetComponent<cardEffect>();
        uiScript.setDefaultPositions();
    }

    public void startCombat()
    {
        isTutorial = DialogueManager.GetInstance().tutorialStage < 3;

        //isPlayerCursed = true;
        combatParent.SetActive(true);
        victoryParent.SetActive(false);

        setStats();
        if(DialogueManager.GetInstance().result == "")
        {
            DialogueManager.GetInstance().ClearAll();
            uiScript.resetUIs();
            discardPile.Clear();
            drawPile.Clear();
            playerHand.Clear();
        }
        TextMovement.GetInstance().ResetPos();
        DialogueManager.GetInstance().EnterNarration();

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
        card currentCard = opponentCards[Random.Range(0, opponentCards.Count)]; // CHANGE AFTER ADDING RETORT
        if (isOpponentCursed)
        {
            currentCard = cursedCard;

        } else if (isTutorial)
        {
            currentCard = opponentCards[tutorialCardCycle];
            tutorialCardCycle++;

            if (tutorialCardCycle == 3)
            {
                tutorialCardCycle = 0;
            }
        }

        DialogueManager.GetInstance().EnterCombat("Opponent", currentCard.type.ToString());

        
        switch (currentCard.type)
        {
            case card.cardType.Attack:
                inflictDamage(0, currentCard.cardStrength);
                subwayUI.instance.setGuideTextPerm("Argued for " + (currentCard.cardStrength+bonusOpponentAttack) + " Willpower");
                break;

            case card.cardType.Defend:
                retort(1, currentCard.cardStrength);
                subwayUI.instance.setGuideTextPerm("Retorted for " + (currentCard.cardStrength+bonusOpponentDefend) + " Willpower");
                break;

            case card.cardType.Effect:
                effectsScript.playCard(currentCard, 1);
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
                subwayUI.instance.setGuideTextPerm("Argued for " + (cardInfo.cardStrength+bonusPlayerAttack) + " Willpower");
                break;

            case card.cardType.Defend:
                retort(0, cardInfo.cardStrength);
                subwayUI.instance.setGuideTextPerm("Retorted for " + (cardInfo.cardStrength+bonusPlayerDefend) + " Willpower");
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
        if (!isTutorial)
        {
            nodeManager.instance.progressStation();
            npcManagerScript.updateCar();
        }

        bonusOpponentAttack = 0;
        bonusPlayerAttack = 0;
        bonusOpponentDefend = 0;
        bonusPlayerDefend = 0;

        uiScript.clearCards();
        gameManager.instance.playerHealth = playerHealth;

        combatParent.SetActive(false);

        uiScript.updateHealthUI(1, 1, 1, playerHealth, playerMaxHealth, tempPlayerHealth);
        if (state == "loss")
        {
            npcManagerScript.removeAll();
            subwayUI.instance.refreshUI(cardsPlayed * cardTimeMultiplier, 2);

            DialogueManager.GetInstance().result = "lose";
            startCombat();

            //subwayManager.instance.switchToMovement();
        }

        if (state == "victory")
        { 
            gameManager.instance.followerAmount++;
            subwayUI.instance.refreshUI(cardsPlayed * cardTimeMultiplier, 2);

            DialogueManager.GetInstance().result = "win";
            startCombat();

            /*subwayUI.instance.setGuideTextPerm("Replace at least one card!");
            victoryParent.SetActive(true);
            victoryCardSpawner.spawnNewCards(opponent.aggression);*/
        }
    }

    private void createOpponentDeck()
    {
        opponentCards.Clear();
        for (int i = 0; i < opponentDeckSize; i++)
        {
            card newCard = ScriptableObject.CreateInstance<card>();
            float randomFloat = Random.Range(0f, 1f);
            if (i < 3) //effect
            {
                switch (nodeManager.instance.currentLine)
                {
                    case "pilgrim":
                        if (randomFloat < 0.5f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[0];
                            newCard.cardStrength = Mathf.RoundToInt(Random.Range(opponent.attack - 2, opponent.attack + 3) * 1.5f);
                        } else
                        {
                            newCard = gameManager.instance.effectCardTemplates[1];
                        }
                        break;

                    case "gallium":
                        if (randomFloat < 0.33f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[2];
                        } else if (randomFloat <0.66) 
                        {
                            newCard = gameManager.instance.effectCardTemplates[3];
                            newCard.cardStrength = Random.Range(1, 3);
                        } else
                        {
                            newCard = gameManager.instance.effectCardTemplates[4];
                            newCard.cardStrength = Mathf.RoundToInt(Random.Range(opponent.defense - 2, opponent.defense + 3) * 0.75f);
                        }
                        break;

                    case "pulse":
                        if (randomFloat < 0.33f)
                        {
                            newCard = gameManager.instance.effectCardTemplates[5];
                            newCard.cardStrength = Random.Range(1, 5);
                        }
                        else if (randomFloat < 0.66)
                        {
                            newCard = gameManager.instance.effectCardTemplates[6];
                            newCard.cardStrength = Random.Range(opponent.defense - 2, opponent.defense + 3);
                        }
                        else
                        {
                            newCard = gameManager.instance.effectCardTemplates[7];
                            newCard.cardStrength = Mathf.RoundToInt(Random.Range(opponent.attack - 2, opponent.attack + 3) * 0.33f);
                        }
                        break;
                }

            } else if (i < 8) //attack
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

        if (isTutorial)
        {
            card card0 = ScriptableObject.CreateInstance<card>();
            card0.type = card.cardType.Attack;
            card0.cardStrength = opponent.attack;
            opponentCards[0] = card0;

            card card1 = ScriptableObject.CreateInstance<card>();
            card1.type = card.cardType.Defend;
            card1.cardStrength = opponent.defense;
            opponentCards[1] = card1;

            card card2 = ScriptableObject.CreateInstance<card>();
            card2 = gameManager.instance.effectCardTemplates[2];
            opponentCards[2] = card2;
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

    public void healthGuideText(int target)
    {
        if (subwayManager.instance.state != "combat") tempPlayerHealth = gameManager.instance.playerHealth;
        if (target == 0)
        {
            subwayUI.instance.setGuideTextTemp("Your Willpower: " + tempPlayerHealth + "/" + playerMaxHealth);
        }
        else
        {
            subwayUI.instance.setGuideTextTemp("Opponent Willpower: " + tempOpponentHealth + "/" + opponentMaxHealth);
        }
    }
}
