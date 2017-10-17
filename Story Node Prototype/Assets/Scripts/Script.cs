using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Holds/manages the current story state (set it). Presents content and choices. Then allows you to make choices.
/// </summary>
public class Script : MonoBehaviour {

    public TextAsset inkAsset;  //Compiled JSON asset

    [SerializeField] private Story _inkStory;    //The story

    public string storyLocation
    {
        set
        {
            _inkStory.ChoosePathString(value);
        }
    }

    bool choiceNeeded = false;

    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);   //The JSON string from the story
    }

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

    private void Update()
    {
        //3 Make choice
        if (choiceNeeded)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _inkStory.ChooseChoiceIndex(0);
                DoStory();
                choiceNeeded = false;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _inkStory.ChooseChoiceIndex(1);
                DoStory();
                choiceNeeded = false;
            }
        }
    }
}
