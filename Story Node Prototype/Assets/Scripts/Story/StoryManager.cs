﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

/// <summary>
/// Singleton.
/// Wrapper class for Ink Story/Runtime
/// Holds references to Ink assets and Game assets that directly interact with ink
/// Holds/manages the current story state (set it). Presents content and choices, and then it allows you to make choices.
/// </summary>
public class StoryManager : MonoBehaviour {

    // ----- SINGLETON ----- //
    private static StoryManager _StoryManager= null;

    // ----- INK RUNTIME ----- //
    [SerializeField] private Story inkStory;    //The story (ink story/script)
    [SerializeField] private TextAsset inkAsset;  //Compiled JSON asset
    [SerializeField] private TextAsset savedAsset;  //Saved ink story state - choices, vars, lists, etc.

    // ----- PROPERTIES ----- //
    public Story InkScript
    {
        get { return inkStory; }
        private set { inkStory = value; }
    }

    public string StoryPosition
    {
        set { inkStory.ChoosePathString(value); }
    }

    // ----- GAME & STATE ----- //
    //UI
    public UIController userInterface;

    //World Display
    public GameObject displayStoryObject;    //Reference to dialogue box - for world pos
    private Text displayStoryText;    //Story object's text component - for displaying text

    //Ink Logic
    private bool choiceMade = false;
    private bool choiceNeeded = false;
    private bool choicesPresented = false;
    

    private void Awake()
    {
        #region Singleton
        if (_StoryManager == null)
        {
            _StoryManager = this;
        }else if(_StoryManager != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        #endregion

        inkStory = new Story(inkAsset.text);   //The JSON string from the story

        displayStoryText = displayStoryObject.gameObject.transform.Find("StoryText").GetComponent<Text>();
    }

    /// <summary>
    /// Makes calls to the ink story in a loop. There are two repeating stages:
    /// 1. Present Content
    /// 2. Present Choices
    /// Followed by: 3. Make Choice
    /// </summary>
    public void DoStory()
    {
        //1. Present content
        while (inkStory.canContinue)
        {
            displayStoryText.text = inkStory.Continue();
        }

        //2. Present choices
        if (choicesPresented == false)
        {
            if (inkStory.currentChoices.Count > 0)
            {
                int numChoices = inkStory.currentChoices.Count;
                choiceNeeded = true;
                userInterface.EnableChoiceWindow(numChoices);

                for (int i = 0; i < numChoices; i++)
                {
                    Choice choice = inkStory.currentChoices[i];
                    userInterface.PresentChoice((i + 1) + ": " + choice.text, i);
                }
                choicesPresented = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //3 Make choice
        if (choiceNeeded)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                inkStory.ChooseChoiceIndex(0);
                choiceNeeded = false;
                choicesPresented = false;
                userInterface.DisableChoiceWindow();
                DoStory();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                inkStory.ChooseChoiceIndex(1);
                choiceNeeded = false;
                choicesPresented = false;
                userInterface.DisableChoiceWindow();
                DoStory();
            }
        }
    }
}
