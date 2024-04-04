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

    [SerializeField] GameObject[] textBoxes;
    [SerializeField] TextMeshProUGUI[] textBoxesText = new TextMeshProUGUI[4];

    [SerializeField] Sprite enemyBubble;
    [SerializeField] Sprite playerBubble;

    [SerializeField] TextAsset inkFile;

    private Story currentStory;
    private DialogueVariables dialogueVariables;

    private static CombatDialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(inkFile);

        for(int i = 0; i < textBoxes.Length; i++)
        {
            textBoxesText[i] = textBoxes[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public static CombatDialogueManager GetInstance()
    {
        return instance;
    }

    public void EnterCombat(string character, string action)
    {
        TextMovement.GetInstance().moveBoxes();
        currentStory = new Story(inkFile.text);
        if (action != "Defend")
        {
            //TextMovement.GetInstance().moveBoxes();
            currentStory.ChoosePathString(character + "." + action);
            dialogueVariables.StartListening(currentStory);
            if (character == "Player")
            {
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        break;
                    }
                }
                //playerTextBox.text = currentStory.Continue();
                //StartCoroutine(DisplayLine(playerTextBox, currentStory.Continue()));
                //playerDefense = "";
                //enemyTextBox.text = "";
                enemyDefense = currentStory.Continue();
            }
            else if (character == "Opponent")
            {
                //enemyTextBox.text = currentStory.Continue();
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        StartCoroutine(DisplayLine(textBoxesText[i], currentStory.Continue()));
                        break;
                    }
                }
                //StartCoroutine(DisplayLine(enemyTextBox, currentStory.Continue()));
                //enemyDefense = "";
                //playerTextBox.text = "";
                playerDefense = currentStory.Continue();
            }
        }


        else if (action == "Defend")
        {
            //TextMovement.GetInstance().moveBoxes();
            if (character == "Player")
            {
                //playerTextBox.text = playerDefense;
                //StartCoroutine(DisplayLine(playerTextBox, playerDefense));
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        StartCoroutine(DisplayLine(textBoxesText[i], playerDefense));
                        break;
                    }
                }
            }
            else if (character == "Opponent")
            {
                //enemyTextBox.text = enemyDefense;
                //StartCoroutine(DisplayLine(enemyTextBox, enemyDefense));
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    if (textBoxesText[i].text == "")
                    {
                        StartCoroutine(DisplayLine(textBoxesText[i], enemyDefense));
                        break;
                    }
                }
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
        for (int i = 0; i < textBoxes.Length; i++)
        {
            if (textBoxesText[i].text != "")
            {
                textBoxesText[i].text = "";
            }
        }
    }
}
