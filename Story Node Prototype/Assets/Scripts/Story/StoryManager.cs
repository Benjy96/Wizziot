using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private TextAsset savedAsset;

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

    // ----- STATE ----- //
    public GameObject displayStoryAsset;    //Reference to dialogue box
    private bool choiceNeeded = false;
    

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
            Debug.Log(inkStory.Continue());
        }

        //2. Present choices
        if (inkStory.currentChoices.Count > 0)
        {
            choiceNeeded = true;
            for (int i = 0; i < inkStory.currentChoices.Count; i++)
            {
                Choice choice = inkStory.currentChoices[i];
                Debug.Log("Choice " + (i + 1) + ". " + choice.text);
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
                DoStory();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                inkStory.ChooseChoiceIndex(1);
                choiceNeeded = false;
                DoStory();
            }
        }
    }
}
