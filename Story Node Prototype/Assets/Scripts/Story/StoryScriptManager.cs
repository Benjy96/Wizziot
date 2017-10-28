using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Singleton.
/// Wrapper class for Ink Story/Runtime
/// Holds references to Ink assets and Game assets that directly interact with ink
/// Holds/manages the current story state (set it).
/// </summary>
public class StoryScriptManager : MonoBehaviour {

    // ----- INK RUNTIME ----- //
    [SerializeField] private Story inkStory;    //The story (ink story/script)
    [SerializeField] private TextAsset inkAsset;  //Compiled JSON asset
    [SerializeField] private TextAsset savedAsset;  //Saved ink story state - choices, vars, lists, etc.

    private void Awake()
    {
        inkStory = new Story(inkAsset.text);   //The JSON string from the story
    }

    public Story InkScript
    {
        get { return inkStory; }
        private set { inkStory = value; }
    }

    public bool ContentAvailable
    {
        get { return inkStory.canContinue; }
    }

    public bool ChoicesAvailable
    {
        get { return inkStory.currentChoices.Count > 0; }
    }

    public int NumChoicesAvailable
    {
        get { return inkStory.currentChoices.Count; }
    }

    public string StoryPosition
    {
        set { inkStory.ChoosePathString(value); }
    }
    
    public string GetChoice(int availableChoiceIndex)
    {
        Choice choice = inkStory.currentChoices[availableChoiceIndex];
        return choice.text;
    }

    public void MakeChoice(int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
    }

    public string GetContent
    {
        get {
            string test = inkStory.Continue();
            Debug.Log(test);
            return test; }
    }
}