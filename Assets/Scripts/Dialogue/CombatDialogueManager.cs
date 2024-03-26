using Ink.Runtime;
using TMPro;
using UnityEngine;

public class CombatDialogueManager : MonoBehaviour
{
    public string enemyText;
    public string enemyDefense;

    public string playerText;
    public string playerDefense;

    public TextMeshProUGUI enemyTextBox;
    public TextMeshProUGUI playerTextBox;

    public TextAsset inkFile;

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
                playerTextBox.text = currentStory.Continue();
                enemyDefense = currentStory.Continue();
            }
            else if (character == "Opponent")
            {
                enemyTextBox.text = currentStory.Continue();
                playerDefense = currentStory.Continue();
            }
        }


        else if (action == "Defend")
        {
            if (character == "Player")
            {
                playerTextBox.text = playerDefense;
            }
            else if (character == "Opponent")
            {
                enemyTextBox.text = enemyDefense;
            }
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
