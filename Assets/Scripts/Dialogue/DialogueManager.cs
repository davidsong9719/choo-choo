using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Security;
using static UnityEngine.Rendering.DebugUI;
using static Cinemachine.DocumentationSortingAttribute;

public class DialogueManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float typingSpeed = 0.04f;


    [Header("UI")]
    //public Animator portraitAnimator;

    [SerializeField] GameObject[] textBoxes;
    [SerializeField] TextMeshProUGUI[] textBoxesText = new TextMeshProUGUI[4];
    [SerializeField] GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    [SerializeField] Sprite enemyBubble;
    [SerializeField] Sprite playerBubble;

    [Header("Ink Files")]
    [SerializeField] TextAsset narrationDialogue;
    [SerializeField] TextAsset combatDialogue;
    [SerializeField] TextAsset inkFile;

    [Header("Dialogue")]
    [SerializeField] string enemyText;
    [SerializeField] string enemyDefense;

    [SerializeField] string playerText;
    [SerializeField] string playerDefense;

    //[HideInInspector] public int tutorialStage;
    public int tutorialStage;
    public string result;

    //[HideInInspector] public string result;


    private Story currentStory;
    public bool narrationIsPlaying { get; private set; }

    //check if player can move to the next line - prevent skipping
    [HideInInspector] public bool canContinueToNextLine = true;

    //private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private string character;

    //public PlayerInputActions playerControls;
    private InputAction submit;

    //used to check if pressed this frame, used in coroutine
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private DialogueVariables dialogueVariables;

    private string nextLine;

    [HideInInspector] public Color bubbleColor;

    private void Awake()
    {
        tutorialStage = 1;

        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;

        for (int i = 0; i < textBoxes.Length; i++)
        {
            textBoxesText[i] = textBoxes[i].GetComponentInChildren<TextMeshProUGUI>();
            textBoxes[i].GetComponentInChildren<Image>().enabled = false;
        }

        //set up whether we use the narration or combat scripts
        inkFile = narrationDialogue;
        dialogueVariables = new DialogueVariables(inkFile);
    }

    private void OnEnable() //Must be used for the new input system to work properly
    {
        submit = subwayManager.instance.playerControls.Player.Interact;
        submit.Enable();
    }

    private void OnDisable() //Must be used for the new input system to work properly
    {
        submit.Disable();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        narrationIsPlaying = false;

        //get all of the coices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        for (int i = 0; i < textBoxes.Length; i++)
        {
            textBoxesText[i] = textBoxes[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }


    public void EnterNarration()
    {
        inkFile = narrationDialogue;
        dialogueVariables = new DialogueVariables(inkFile);
        currentStory = new Story(inkFile.text);
        narrationIsPlaying = true;
        

        if (tutorialStage == 1)
        {
            currentStory.ChoosePathString("Tutorial");
        }
        else if (tutorialStage == 2)
        {
            if (result == "")
            {
                currentStory.ChoosePathString("Tutorial2");
            } 
        }
        else if (tutorialStage == 3)
        {
            currentStory.ChoosePathString("Tutorial3");
        }
        else if (tutorialStage == 5)
        {
            ((IntValue)GetVariableState("score")).value = gameManager.instance.followerAmount;
            Debug.Log("score is " + ((IntValue)GetVariableState("score")).value);

            if(PlayerPrefs.GetInt("lastScore") != 0)
            {
                ((IntValue)GetVariableState("lastScore")).value = PlayerPrefs.GetInt("lastScore");
                Debug.Log("last score is " + ((IntValue)GetVariableState("lastScore")).value);
            }
            if (PlayerPrefs.GetInt("highScore") != 0)
            {
                ((IntValue)GetVariableState("highScore")).value = PlayerPrefs.GetInt("highScore");
                Debug.Log("high score is " + ((IntValue)GetVariableState("highScore")).value);
            }

            currentStory.ChoosePathString("Ending");
        }
        else
        {
            currentStory.ChoosePathString("Talk");
        }
        

        if(result == "win")
        {
            if (tutorialStage > 3)
            {
                currentStory.ChoosePathString("Win");
            }
            else
            {
                currentStory.ChoosePathString("TutorialWin");
            }
            
        }
        if(result == "lose")
        {
            if (tutorialStage > 3)
            {
                currentStory.ChoosePathString("Lose");
            }
            else
            {
                currentStory.ChoosePathString("TutorialLose");
            }
        }

        //if we change the portraits (we prolly wont but in case ig)
        //portraitAnimator.Play("");

        dialogueVariables.StartListening(currentStory);

        ContinueStory();
    }

    //Swap to combat
    private void ExitNarration()
    {
        narrationIsPlaying = false;
        inkFile = combatDialogue;

        if (result == "win")
        {
            subwayUI.instance.setGuideTextPerm("Replace at least one card!");
            combatManager.instance.victoryParent.SetActive(true);
            combatManager.instance.victoryCardSpawner.spawnNewCards(combatManager.instance.opponent.aggression);
            result = "";
            ClearAll();
        }
        else if (result == "lose")
        {
            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToMovement));
            result = "";
            //ClearAll();
        }
        else
        {
            if (tutorialStage == 4 || tutorialStage == 2)
            {
                dialogueVariables = new DialogueVariables(inkFile);
                combatManager.instance.fight();
            }
        }


        if (tutorialStage == 1)
        {
            tutorialStage = ((IntValue)GetVariableState("tutorialStage")).value;

            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToMovement));
        }
        else if (tutorialStage == 3)
        {
            tutorialStage = 4;

            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToMovement));
        }
        else if (tutorialStage == 5)
        {
            if (((IntValue)GetVariableState("score")).value > ((IntValue)GetVariableState("highScore")).value)
            {
                ((IntValue)GetVariableState("highScore")).value = ((IntValue)GetVariableState("score")).value;
                PlayerPrefs.SetInt("highScore", ((IntValue)GetVariableState("highScore")).value);
            }
            ((IntValue)GetVariableState("lastScore")).value = ((IntValue)GetVariableState("score")).value;
            PlayerPrefs.SetInt("lastScore", ((IntValue)GetVariableState("lastScore")).value);
            StartCoroutine(TransitionManager.GetInstance().End());
        }


    }


    void Update()
    {

        //return right away if dialogue isnt playing
        if (!narrationIsPlaying)
        {
            return;
        }


        //continue to next line when submit is pressed
        if (currentStory.currentChoices.Count == 0 && canContinueToNextLine && submit.WasPressedThisFrame())
        {
            ContinueStory();
        }

    }

    private void ContinueStory()
    {
        if (currentStory.canContinue && canContinueToNextLine == true)
        {
            if (TextMovement.GetInstance().inPlace == true)
            {
                canContinueToNextLine = false;
                TextMovement.GetInstance().moveBoxes();
                //set text for the current dialogue line
                nextLine = currentStory.Continue();

                //display choices if there are any
                DisplayChoices();
                //handle tags
                HandleTags(currentStory.currentTags);


                //start coroutine to type one letter at a time
                if (character == "Player")
                {
                    for (int i = 0; i < textBoxes.Length; i++)
                    {
                        if (textBoxesText[i].text == "")
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], nextLine));
                            break;
                        }
                    }
                    //displayLineCoroutine = StartCoroutine(DisplayLine(playerTextBox, nextLine));
                }
                else if (character == "Opponent")
                {
                    for (int i = 0; i < textBoxes.Length; i++)
                    {
                        if (textBoxesText[i].text == "")
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], nextLine));
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("not working");
                }

            }
        }
        else
        {
            ExitNarration();
        }
    }

    private IEnumerator DisplayLine(TextMeshProUGUI textBox, string line)
    {
        textBox.text = line;
        textBox.maxVisibleCharacters = 0;

        Image bubbleImage = textBox.GetComponentInParent<Image>();
        bool isPlayer = bubbleImage.sprite == playerBubble;
        if (isPlayer)
        {
            isPlayer = true;
            bubbleImage.color = Color.white;
        } else
        {
            bubbleImage.color = bubbleColor;
        }   
        
        //hide choices while typing
        HideChoices();

        canContinueToNextLine = false;
        submit.Disable();

        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            //check for text tag, dont show (wait for characters) if <>
            textBox.maxVisibleCharacters++;

            if (isPlayer)
            {
                GetComponent<dialogueAudio>().makeNoise(0);
            } else
            {
                GetComponent<dialogueAudio>().makeNoise(1);
            }
            
            yield return new WaitForSeconds(typingSpeed);

        }

        //display choices if there are any
        DisplayChoices();
        if (textBox.maxVisibleCharacters == line.Length)
        {
            canContinueToNextLine = true;
            submit.Enable();
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        Debug.Log(currentTags.Count);
        //loop through all tags and handle them
        foreach (string tag in currentTags)
        {
            //parse the tags, split string into 2 parts - key and value
            string[] splitTag = tag.Split('-');
            Debug.Log(splitTag.Length);
            //defensive programming to double check for errors
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            Debug.Log(tagKey);

            //handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    character = tagValue;
                    break;
                case PORTRAIT_TAG:
                    //portraitAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being hangled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //make sure the choices dont go over limit
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("too many choices for UI to support");
        }

        int index = 0;
        //enable and initialize choices up to the amount of choices for this line
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        //go though the remaining choices that the UI supports and make sure theyre hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        //Event system needs to be cleared
        //wait a frame before selecting the currently selected object
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }

    }

    //==============
    //COMBAT
    //==============



    public void EnterCombat(string character, string action)
    {
        if (TextMovement.GetInstance().inPlace == true)
        {
            TextMovement.GetInstance().moveBoxes();
        }
        
        currentStory = new Story(inkFile.text);
        if (tutorialStage == 2)
        {
            currentStory.ChoosePathString(character + "Tutorial." + action);
        }
        else
        {
            currentStory.ChoosePathString(character + "." + action);
        }
        dialogueVariables.StartListening(currentStory);
        if (action != "Defend")
        {
            if (character == "Player")
            {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                        textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        break;
                    }
                }
                playerDefense = "";
                enemyDefense = currentStory.Continue();
            }
            else if (character == "Opponent")
            {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                        textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        break;
                    }
                }
                enemyDefense = "";
                playerDefense = currentStory.Continue();
            }
        }


        else if (action == "Defend")
        {
            if (character == "Player")
            {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        if (playerDefense != "")
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], playerDefense));
                            playerDefense = "";
                            enemyDefense = "";
                            break;
                        }
                        else
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                            enemyDefense = "";
                            break;
                        }
                    }
                }
            }
            else if (character == "Opponent")
            {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        if (enemyDefense != "")
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], enemyDefense));
                            enemyDefense = "";
                            playerDefense = "";
                            break;
                        }
                        else
                        {
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
                            StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                            playerDefense = "";
                            break;
                        }
                    }
                }
            }
        }
    }


    public void ClearAll()
    {
        for (int i = 0; i < textBoxes.Length; i++)
        {
            if (textBoxesText[i].text != "")
            {
                textBoxesText[i].text = "";
                textBoxes[i].GetComponentInChildren<Image>().enabled = false;
            }
        }
        enemyDefense = "";
        playerDefense = "";


        //return to regular dialogue
        //inkFile = narrationDialogue;
        //dialogueVariables = new DialogueVariables(inkFile);
    }



    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink variables was found to be null: " + variableName);
        }
        return variableValue;
    }

}