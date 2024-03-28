//using Ink.Parsed;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
//using Ink.UnityIntegration;

public class DialogueManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float typingSpeed = 0.04f;


    [Header("Dialogue UI")]
    [SerializeField] TextMeshProUGUI enemyTextBox;
    [SerializeField] TextMeshProUGUI playerTextBox;
    //public Animator portraitAnimator;

    [Header("Globals Ink File")]
    [SerializeField] TextAsset inkFile;

    [Header("Choices UI")]
    public GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    //check if player can move to the next line - prevent skipping
    private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private string character;

    public PlayerInputActions playerControls;
    private InputAction submit;

    //used to check if pressed this frame, used in coroutine
    private bool submitPressed = false;


    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private DialogueVariables dialogueVariables;

    public string nextLine;




    private void Awake()
    {
        playerControls = new PlayerInputActions();

        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(inkFile);
    }

    private void OnEnable() //Must be used for the new input system to work properly
    {
        submit = playerControls.Player.Interact;
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
        dialogueIsPlaying = false;

        //get all of the coices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }


    public void EnterDialogueMode()
    {
        currentStory = new Story(inkFile.text);
        dialogueIsPlaying = true;

        dialogueVariables.StartListening(currentStory);


        //reset back to default
        //portraitAnimator.Play("");

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        enemyTextBox.text = "";
        playerTextBox.text = "";

        combatManager.instance.fight();


    }

    // Update is called once per frame
    void Update()
    {

        if (submit.WasPressedThisFrame())
        {
            submitPressed = true;
        }

        //return right away if dialogue isnt playing
        if (!dialogueIsPlaying)
        {
            return;
        }


        //continue to next line when submit is pressed
        if (currentStory.currentChoices.Count == 0 && canContinueToNextLine && submit.WasPressedThisFrame())
        {
            submitPressed = false;
            ContinueStory();

        }

    }

    private void ContinueStory()
    {
        submitPressed = false;
        if (currentStory.canContinue)
        {
            //prevent 2 coroutines from going at once
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            //set text for the current dialogue line
            //dialogueText.text = currentStory.Continue();
            nextLine = currentStory.Continue();

            //display choices if there are any
            //DisplayChoices();
            //handle tags
            HandleTags(currentStory.currentTags);
            

            //start coroutine to type one letter at a time
            if (character == "Player")
            {
                displayLineCoroutine = StartCoroutine(DisplayLine(playerTextBox, nextLine));
            }
            else if (character == "Opponent")
            {
                displayLineCoroutine = StartCoroutine(DisplayLine(enemyTextBox, nextLine));
            }
            else
            {
            Debug.Log("not working");
            }

        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(TextMeshProUGUI textBox, string line)
    {
        //empty dialogue text
        //dialogueText.text = "";
        Debug.Log(line);
        textBox.text = line;
        textBox.maxVisibleCharacters = 0;

        //hide choices while typing
        HideChoices();

        canContinueToNextLine = false;


        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {

            if (submitPressed)
            {
                submitPressed = false;
                textBox.maxVisibleCharacters = line.Length;
                break;
            }

            //check for text tag, dont show (wait for characters) if <>
            textBox.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);

        }

        //display choices if there are any
        DisplayChoices();
        canContinueToNextLine = true;
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
        //start coroutine to type one letter at a time
        /*if (character == "Player")
        {
            displayLineCoroutine = StartCoroutine(DisplayLine(playerTextBox, currentStory.Continue()));
        }
        else if (character == "Opponent")
        {
            displayLineCoroutine = StartCoroutine(DisplayLine(enemyTextBox, currentStory.Continue()));
        }
        else
        {
            Debug.Log(character);
        }*/
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