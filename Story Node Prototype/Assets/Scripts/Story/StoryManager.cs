using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StoryManager sets up Story I/O, controls its flow, & allows access to get/set its state
/// </summary>
public class StoryManager : MonoBehaviour {

    //Singleton & accessor
    private static StoryManager _StoryManager = null;
    public static StoryManager Instance { get { return _StoryManager; } }

    //Story Manager Components
    private StoryScriptManager scriptManager;
    private StoryDisplayManager displayManager; //TODO: investigate multiple text displays - i.e. multiple canvases displayed concurrently
    private StoryInterfaceManager interfaceManager;

    //State Variables
    private string gameDataFileName;
    private string filepath;
    private string storyState_JSON;

    private InteractableNPC conversationTarget;

    private IEnumerator ExitConversation;  //Prevents the coroutine closing the display if a new conversation has started within the disable UI time delay since last convo
    private bool storyDisplayActive = false;
    private bool storyChoiceDisplayActive = false;
    private bool takeStoryInput = false;    //This variable is accessed concurrently

    private void Awake()
    {
        //Singleton setup
        if (_StoryManager == null)
        {
            _StoryManager = this;
        }
        else if (_StoryManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        gameDataFileName = "wizziot.json";
        filepath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

    //Set up Coroutine to close story on a time delay
    ExitConversation = DisableStoryOnDelay();

        //Get references to the story manager components
        scriptManager = GetComponent<StoryScriptManager>();
        displayManager = GetComponent<StoryDisplayManager>();
        interfaceManager = GetComponent<StoryInterfaceManager>();
    }

    public void BindExternalFunction(string functionToBind, Action UnityFunction)
    {
        try
        {
            scriptManager.InkScript.ValidateExternalBindings();
        }
        #pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception e)
        #pragma warning restore CS0168 // Variable is declared but never used
        {
            scriptManager.InkScript.BindExternalFunction(functionToBind, () => UnityFunction());
        }
    }

    public void AttemptToConverse(InteractableNPC targetNPC)
    {
        StopCoroutine(ExitConversation);
        ExitConversation = DisableStoryOnDelay();   //Resubscribe the variable, otherwise null once stopped
        ResetStoryInterface();

        //Find out if NPC has "anything to say"
        conversationTarget = targetNPC;
        scriptManager.StoryPosition = targetNPC.inkPath;

        if (scriptManager.ContentAvailable)
        {
            Converse();
        }
    }

    private void ResetStoryInterface()
    {
        HideStory();
        DisableInput();
    }

    private void Converse()
    {
        //Set up output display
        if (scriptManager.ContentAvailable && storyDisplayActive == false)
        {
            displayManager.EnableStoryDisplay(conversationTarget.transform);
            displayManager.DisplayPosition = conversationTarget.transform;
            storyDisplayActive = true;
        }

        //INK: 1. Present Story
        if (scriptManager.ContentAvailable && storyDisplayActive == true)
        {
            PresentStory();
        }

        //Set up choice window
        if (scriptManager.ChoicesAvailable && storyChoiceDisplayActive == false)
        {
            interfaceManager.DisplayButtons(scriptManager.NumChoicesAvailable);
            storyChoiceDisplayActive = true;
        }

        //bool if(storyChoiceActive && choicesAvailable && buttonsAlreadyDisplayed)

        //INK: 2. Make Choices
        if (scriptManager.ChoicesAvailable && storyChoiceDisplayActive == true)
        {
            EnableInput();  //We need an UPDATE input beneath this if display active but buttons different
            //e.g. what happens if we have 3 choices after picking from 2? draw flowchart
        }

        //If no content or choices available, end the conversation
        if (!scriptManager.ContentAvailable && !scriptManager.ChoicesAvailable)
        {
            StartCoroutine(ExitConversation);
        }
    }

    private void PresentStory()
    {
        while (scriptManager.ContentAvailable)
        {
            displayManager.DisplayedStoryText = scriptManager.GetContent;
        }
    }

    private void HideStory()
    {
        if (storyDisplayActive)
        {
            displayManager.DisableStoryDisplay(gameObject.transform);
            storyDisplayActive = false;
        }
    }

    private void EnableInput()
    {
        for (int i = 0; i < scriptManager.NumChoicesAvailable; i++)
        {
            interfaceManager.PresentChoice(scriptManager.GetChoice(i), i);
        }
        takeStoryInput = true;
    }

    private void DisableInput()
    {
        takeStoryInput = false;
        interfaceManager.DisableChoiceWindow();
        storyChoiceDisplayActive = false;
    }
   
    private IEnumerator DisableStoryOnDelay()
    {
        Debug.Log("Coroutine method");
        DisableInput();
        yield return new WaitForSeconds(5f);
        HideStory();
    }

    private void Update()
    {
        //INK: 3. Make Choice and Loop Story.
        if (takeStoryInput)
        {
            int playerChoice;
            bool correctInput = int.TryParse(Input.inputString, out playerChoice);
            if (correctInput)
            {
                Choice(playerChoice);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitConversation);
        }
    }

    public void Choice(int playerChoice)
    {
        if (takeStoryInput)
        {
            switch (playerChoice)
            {
                case 1:
                case 2:
                case 3:
                    scriptManager.MakeChoice(playerChoice - 1);
                    if (scriptManager.ContentAvailable)
                    {
                        Converse(); //Story runs in a loop (enabled by bool - takestoryinput) - bool in update
                    }
                    break;
            }
        }
    }

    public void SaveGame()
    {
        storyState_JSON = scriptManager.InkScript.state.ToJson();
        //TODO: create diff save file name for each file - check num of files already in existence & increment a count variable

        File.WriteAllText(filepath, storyState_JSON);
        Debug.Log("Should have written");
    }

    public void LoadGame()
    {
        if (File.Exists(filepath))
        {
            storyState_JSON = File.ReadAllText(filepath);

            if (!storyState_JSON.Equals(""))
            {
                scriptManager.InkScript.state.LoadJson(storyState_JSON);
            }
        }
    }
}