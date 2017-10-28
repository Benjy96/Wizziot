﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Current story flow:
/// Player targets NPC
/// Begins conversation with keypress - F
///     Story displays set up
///     Run ink story (if available)
///     Get player input TODO: Create separate setup conversation/end conversation methods to make more dynamic
///     Clear story displays - need to change when cleared
///     
/// StoryManager sets up the story's flow, output, and handles input
/// </summary>
public class StoryManager : MonoBehaviour {

    //Singleton & accessor
    private static StoryManager _StoryManager = null;
    public static StoryManager Instance { get { return _StoryManager; } }

    //Story Manager Components
    private StoryScriptManager scriptManager;
    private StoryDisplayManager displayManager; //TODO: investigate multiple text displays - i.e. multiple canvases displayed concurrently
    private StoryInterfaceManager interfaceManager;

    private InteractableNPC conversationTarget;

    private bool playerStartedNewConversation = false;

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

        //Get references to the story manager components
        scriptManager = GetComponent<StoryScriptManager>();
        displayManager = GetComponent<StoryDisplayManager>();
        interfaceManager = GetComponent<StoryInterfaceManager>();
    }

    public StoryScriptManager AccessInk //TODO: find a way to bind external functions here - pass the "BindExternalFunctions" parameters
    {
        get { return scriptManager; }
    }

    public void AttemptToConverse(InteractableNPC targetNPC)
    {
        playerStartedNewConversation = true;
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
            interfaceManager.EnableChoiceWindow(scriptManager.NumChoicesAvailable);
            storyChoiceDisplayActive = true;
        }

        //INK: 2. Make Choices
        if (scriptManager.ChoicesAvailable && storyChoiceDisplayActive == true)
        {
            EnableInput();
        }

        //If no content or choices available, end the conversation
        if (!scriptManager.ContentAvailable && !scriptManager.ChoicesAvailable)
        {
            StartCoroutine(ExitConversation());
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
        if (storyDisplayActive && playerStartedNewConversation == false)
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
        if (storyChoiceDisplayActive)
        {
            interfaceManager.DisableChoiceWindow();
            storyChoiceDisplayActive = false;
        }
    }
   
    private IEnumerator ExitConversation()
    {
        playerStartedNewConversation = false;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitConversation());
        }
    }
}