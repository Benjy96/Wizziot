using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class Script : MonoBehaviour {

    public TextAsset inkAsset;  //Compiled JSON asset
    public Story _inkStory;    //The story
    bool choiceNeeded = false;

    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);   //The JSON string from the story
    }

    public void DoStory()
    {
        //Present content
        while (_inkStory.canContinue)
        {
            Debug.Log(_inkStory.Continue());
        }

        //Present Choices
        if(_inkStory.currentChoices.Count > 0)
        {
            for (int i = 0; i < _inkStory.currentChoices.Count; i++)
            {
                Choice choice = _inkStory.currentChoices[i];
                Debug.Log("Choice " + (i + 1) + ". " + choice.text);
            }
        }
    }

    private void Update()
    {
        ////2.1 Make choice
        //if (choiceNeeded)
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        _inkStory.ChooseChoiceIndex(0);
        //        choiceNeeded = false;
        //    }

        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        _inkStory.ChooseChoiceIndex(1);
        //        choiceNeeded = false;
        //    }
        //}
        //else
        //{
        //    //1. Present content
        //    while (_inkStory.canContinue)
        //    {
        //        Debug.Log(_inkStory.Continue());
        //    }

        //    //2. Present choices
        //    if (_inkStory.currentChoices.Count > 0 && choiceNeeded != true)
        //    {
        //        for (int i = 0; i < _inkStory.currentChoices.Count; i++)
        //        {
        //            Choice choice = _inkStory.currentChoices[i];
        //            Debug.Log("Choice " + (i + 1) + ". " + choice.text);
        //        }
        //        choiceNeeded = true;
        //    }
        //}
    }
}
