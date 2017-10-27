using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Singleton.
/// Wrapper class for Ink Story/Runtime
/// Holds references to Ink assets and Game assets that directly interact with ink
/// Holds/manages the current story state (set it).
/// </summary>
public class StoryScript : MonoBehaviour {

    // ----- SINGLETON ----- //
    private static StoryScript _Script = null;

    // ----- INK RUNTIME ----- //
    [SerializeField] private Story inkStory;    //The story (ink story/script)
    [SerializeField] private TextAsset inkAsset;  //Compiled JSON asset
    [SerializeField] private TextAsset savedAsset;  //Saved ink story state - choices, vars, lists, etc.

    public static StoryScript Instance { get { return _Script; } }

    public Story InkScript
    {
        get { return inkStory; }
        private set { inkStory = value; }
    }

    // ----- STATE PROPERTIES ----- //
    public string StoryPosition
    {
        set { inkStory.ChoosePathString(value); }
    }
    
    private void Awake()
    {
        #region Singleton
        if (_Script == null)
        {
            _Script = this;
        }else if(_Script != this){
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
        //1. Present Content
        while (inkStory.canContinue)
        {
            //TODO: Implement
            Debug.Log(inkStory.Continue());
        }

        //2. Present Choices
        if (inkStory.currentChoices.Count > 0)
        {
            int numChoices = inkStory.currentChoices.Count;

            for (int i = 0; i < numChoices; i++)
            {
                Choice choice = inkStory.currentChoices[i];
            }
        }
    }

    //3. Make Choice
    public void MakeChoice(int choice)
    {
        DoStory();
    }
}