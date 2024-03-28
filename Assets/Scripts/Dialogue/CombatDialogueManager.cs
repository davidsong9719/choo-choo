using Ink.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class CombatDialogueManager : MonoBehaviour
{
    [SerializeField] float typingSpeed = 0.04f;

    [SerializeField] string enemyText;
    [SerializeField] string enemyDefense;

    [SerializeField] string playerText;
    [SerializeField] string playerDefense;

    [SerializeField] TextMeshProUGUI enemyTextBox;
    [SerializeField] TextMeshProUGUI playerTextBox;

    [SerializeField] TextAsset inkFile;

    private Story currentStory;
    private DialogueVariables dialogueVariables;
    private Coroutine displayLineCoroutine;

    private static CombatDialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(inkFile);
    }
    public static CombatDialogueManager GetInstance()
    {
        return instance;
    }

    public void EnterCombat(string character, string action)
    {
        currentStory = new Story(inkFile.text);
        if (action != "Defend")
        {
            currentStory.ChoosePathString(character + "." + action);
            dialogueVariables.StartListening(currentStory);
            if (character == "Player")
            {
                //playerTextBox.text = currentStory.Continue();
                StartCoroutine(DisplayLine(playerTextBox, currentStory.Continue()));
                playerDefense = "";
                enemyTextBox.text = "";
                enemyDefense = currentStory.Continue();
            }
            else if (character == "Opponent")
            {
                //enemyTextBox.text = currentStory.Continue();
                StartCoroutine(DisplayLine(enemyTextBox, currentStory.Continue()));
                enemyDefense = "";
                playerTextBox.text = "";
                playerDefense = currentStory.Continue();
            }
        }


        else if (action == "Defend")
        {
            if (character == "Player")
            {
                //playerTextBox.text = playerDefense;
                StartCoroutine(DisplayLine(playerTextBox, playerDefense));
            }
            else if (character == "Opponent")
            {
                //enemyTextBox.text = enemyDefense;
                StartCoroutine(DisplayLine(enemyTextBox, enemyDefense));
            }
        }
    }

    private IEnumerator DisplayLine(TextMeshProUGUI textBox, string line)
    {
        //empty dialogue text
        //dialogueText.text = "";
        textBox.text = line;
        textBox.maxVisibleCharacters = 0;
        

        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {

            /*if (submitPressed)
            {
                submitPressed = false;
                textBox.maxVisibleCharacters = line.Length;
                break;
            }
            //continue as normal
            else
            {
                //dialogueText.text += letter;
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }*/

            //dialogueText.text += letter;
            textBox.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);

        }
    }

    public void ClearAll()
    {
        //dialogueVariables.StopListening(currentStory);
        enemyTextBox.text = "";
        playerTextBox.text = "";

        enemyDefense = "";
        playerDefense = "";

    }
}
