using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.IO;

public class DialogueVariables
{

    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }


    private Story globalVariablesStory;
    private const string saveVariablesKey = "INK_VARIABLES";

    //compile into JSON, because included files dont do so on their own in editor
    //nvm thats outdated, it does compile, only need to send in a TextAsset
    public DialogueVariables(TextAsset inkJSON)
    {
        //create the story
        globalVariablesStory = new Story(inkJSON.text);

        //check if we have saved data
        if (PlayerPrefs.HasKey(saveVariablesKey))
        {
            string jsonState = PlayerPrefs.GetString(saveVariablesKey);
            globalVariablesStory.state.LoadJson(jsonState);
        }

        //compile the story
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void SaveVariables()
    {
        if (globalVariablesStory != null)
        {
            //Load the current state of all of our variables
            VariablesToStory(globalVariablesStory);

            //save data as sting
            //eventually will replace with actual save load
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }

    public void StartListening(Story story)
    {
        //needs to happen before variable observer is assigned
        VariablesToStory(story);

        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        //only maintain variables initialized in globals ink file
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }

        //Debug.Log("variable changed: " + name + " = " + value);
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
