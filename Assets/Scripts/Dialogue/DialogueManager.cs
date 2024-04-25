using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

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
        else
        {
            currentStory.ChoosePathString("Talk");
        }
        

        if(result == "win")
        {
            currentStory.ChoosePathString("Win");
        }
        if(result == "lose")
        {
            currentStory.ChoosePathString("Lose");
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
            subwayManager.instance.switchToMovement();
            result = "";
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
            tutorialStage = 2;

            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToMovement));
        }
        else if (tutorialStage == 3)
        {
            tutorialStage = 4;

            StartCoroutine(TransitionManager.GetInstance().Swipe(subwayManager.instance.switchToMovement));
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
                            StartCoroutine(DisplayLine(textBoxesText[i], nextLine));
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
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
                            StartCoroutine(DisplayLine(textBoxesText[i], nextLine));
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
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

        //hide choices while typing
        HideChoices();

        canContinueToNextLine = false;
        submit.Disable();

        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {

            //check for text tag, dont show (wait for characters) if <>
            textBox.maxVisibleCharacters++;

            if (textBox.GetComponentInParent<Image>().sprite == playerBubble)
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
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                        textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
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
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                        textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
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
                            StartCoroutine(DisplayLine(textBoxesText[i], playerDefense));
                            playerDefense = "";
                            enemyDefense = "";
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
                            break;
                        }
                        else
                        {
                            StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = playerBubble;
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
                            StartCoroutine(DisplayLine(textBoxesText[i], enemyDefense));
                            enemyDefense = "";
                            playerDefense = "";
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
                            break;
                        }
                        else
                        {
                            StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                            textBoxes[i].GetComponentInChildren<Image>().enabled = true;
                            textBoxes[i].GetComponentInChildren<Image>().sprite = enemyBubble;
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

}