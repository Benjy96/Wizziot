using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    //Singleton & accessor
    private static StoryManager _StoryManager = null;
    public static StoryManager Instance { get { return _StoryManager; } }

    //Story Manager Components
    private StoryScriptManager scriptManager;
    private StoryDisplayManager displayManager;
    private StoryInterfaceManager interfaceManager;

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
    
    public void StartConversation(InteractableNPC targetNPC)
    {
        //TODO: Implement conversation
        //1. Check NPC story path (what it wants to say - individual "path" attribute
        //LOOP:    
            
            //2. Check if we can run story
                //INK FLOW:
                //I.1. Present Content
            //3. Display world UI (story output)
                //I.2 Present Choices
            //4. Display Canvas 2D UI (story input canvas)
                //I.3 Make Choices
            //5. Get player input

        //END LOOP

        //6. Remove Canvas and world UI
        //7. Close input streams



    }
}
