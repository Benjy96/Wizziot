using System;
using System.IO;
using System.Collections;
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
    private string storyDataFileName;
    private string filepath;
    private string storyState_JSON;

    public bool StoryInputEnabled { get { return takeStoryInput; } }
    public bool StoryClosing { get { return closingStory; } }

    //Implementation Variables
    private InteractableNPC conversationTarget;
    private IEnumerator ExitConversation;
    private bool closingStory = false;
    private bool storyDisplayActive = false;
    private bool storyChoiceDisplayActive = false;
    private bool takeStoryInput = false;

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

        //Set up Coroutine to close story on a time delay
        ExitConversation = DisableStoryOnDelay();

        //Get references to the story manager components
        scriptManager = GetComponent<StoryScriptManager>();
        displayManager = GetComponent<StoryDisplayManager>();
        interfaceManager = GetComponent<StoryInterfaceManager>();
    }

    private void Start()
    {
        storyDataFileName = GameManager.Instance.StorySaveFileName; //Needs to be in Start since Awake the scripts are still initializing and in random order
        filepath = Path.Combine(Application.streamingAssetsPath, storyDataFileName);

        PlayerManager.Instance.playerControls.OnEscapeKey += CloseConversation;
    }

    #region Interface
    public void BindExternalFunction(string functionToBind, Action UnityFunction)
    {
        try //Using try/catch like an if statement: If all externals not bound, bind UnityFunction
        {
            scriptManager.InkScript.ValidateExternalBindings();
        }
        catch (Exception e)
        {
            scriptManager.InkScript.BindExternalFunction(functionToBind, () => UnityFunction());
        }
    }

    public void AttemptToConverse(InteractableNPC targetNPC)
    {
        if (targetNPC != null && targetNPC.targetType == TargetType.Story)
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
    }

    public void CloseConversation()
    {
        if (storyChoiceDisplayActive && storyDisplayActive)
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
                        Converse();
                    }
                    break;
            }
        }
    }

    public void SaveStory()
    {
        storyState_JSON = scriptManager.InkScript.state.ToJson();
        //TODO: create diff save file name for each file - check num of files already in existence & increment a count variable

        File.WriteAllText(filepath, storyState_JSON);
        Debug.Log("...Saved story state");
    }

    public void LoadStory()
    {
        if (!storyChoiceDisplayActive)
        {
            Debug.Log("... Loading previous story state");
            if (File.Exists(filepath))
            {
                storyState_JSON = File.ReadAllText(filepath);

                if (!storyState_JSON.Equals(""))
                {
                    scriptManager.InkScript.state.LoadJson(storyState_JSON);
                    Debug.Log("Loaded saved story");
                }
            }
        }
    }
    #endregion

    #region Story Implementation
    private void Converse()
    {
        //TODO: If npc doesn't speak first, don't display text box - i.e. if a choice but no content, (on start) - hide text? OR change way ink is written
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

        if (scriptManager.ChoicesAvailable)
        {
            interfaceManager.DisplayButtons(scriptManager.NumChoicesAvailable);
            EnableInput();
            storyChoiceDisplayActive = true;
        }

        //If no content or choices available, end the conversation
        if (!scriptManager.ContentAvailable && !scriptManager.ChoicesAvailable)
        {
            StartCoroutine(ExitConversation);
        }
    }

    private void ResetStoryInterface()
    {
        HideStory();
        DisableInput();
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
        closingStory = true;
        DisableInput();
        yield return new WaitForSeconds(5f);
        HideStory();
        closingStory = false;
    }
    #endregion
}