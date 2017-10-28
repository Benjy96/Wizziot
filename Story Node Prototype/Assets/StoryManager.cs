using UnityEngine;

public class StoryManager : MonoBehaviour {

    //Singleton & accessor
    private static StoryManager _StoryManager = null;
    public static StoryManager Instance { get { return _StoryManager; } }

    //Story Manager Components
    private StoryScriptManager scriptManager;
    private StoryDisplayManager displayManager;
    private StoryInterfaceManager interfaceManager;

    private InteractableNPC conversationTarget;

    private bool storyTextDisplayed = false;
    private bool storyChoicesDisplayed = false;
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
    
    public void Converse(InteractableNPC targetNPC)
    {
        conversationTarget = targetNPC;

        scriptManager.StoryPosition = targetNPC.inkPath;

        if (scriptManager.ContentAvailable && storyTextDisplayed == false)
        {
            displayManager.EnableStoryDisplay(targetNPC.transform);
            displayManager.SetDisplayPosition = targetNPC.transform;

            while (scriptManager.ContentAvailable)
            {
                displayManager.DisplayedStoryText = scriptManager.GetContent;
            }

            storyTextDisplayed = true;
        }

        if(scriptManager.ChoicesAvailable && storyChoicesDisplayed == false)
        {
            interfaceManager.EnableChoiceWindow(scriptManager.NumChoicesAvailable);

            for (int i = 0; i < scriptManager.NumChoicesAvailable; i++)
            {
                interfaceManager.PresentChoice(scriptManager.GetChoice(i), i);
            }

            storyChoicesDisplayed = true;
        }

        if (storyChoicesDisplayed)
        {
            takeStoryInput = true;
        }
    }

    public void ClearConversation()
    {
        if (storyTextDisplayed)
        {
            displayManager.DisableStoryDisplay(gameObject.transform);
            storyTextDisplayed = false;
        }

        if (storyChoicesDisplayed)
        {
            interfaceManager.DisableChoiceWindow();
            storyChoicesDisplayed = false;
        }

        takeStoryInput = false;
    }

    private void Update()
    {
        if (takeStoryInput)
        {
            switch (Input.inputString)
            {
                case "1":
                    scriptManager.MakeChoice(0);
                    if (scriptManager.ContentAvailable) Converse(conversationTarget);
                    ClearConversation();
                    break;
            }
        }
    }
}
