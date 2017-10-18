using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Holds/manages the current story state (set it). Presents content and choices, and then it allows you to make choices.
/// </summary>
public class Script : MonoBehaviour {

    public Story _inkStory;    //The story (ink story/script)

    [SerializeField] private TextAsset inkAsset;  //Compiled JSON asset

    private bool choiceNeeded = false;

    public delegate void NPCExternalFunctions();
    public NPCExternalFunctions PushOff;

    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);   //The JSON string from the story

        _inkStory.BindExternalFunction("PushOff", () => PushOff());
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
        while (_inkStory.canContinue)
        {
            Debug.Log(_inkStory.Continue());
        }

        //2. Present choices
        if (_inkStory.currentChoices.Count > 0)
        {
            choiceNeeded = true;
            for (int i = 0; i < _inkStory.currentChoices.Count; i++)
            {
                Choice choice = _inkStory.currentChoices[i];
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
                _inkStory.ChooseChoiceIndex(0);
                choiceNeeded = false;
                DoStory();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _inkStory.ChooseChoiceIndex(1);
                choiceNeeded = false;
                DoStory();
            }
        }
    }
}
