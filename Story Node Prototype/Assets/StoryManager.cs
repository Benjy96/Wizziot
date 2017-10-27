using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    private StoryScriptManager scriptManager;
    private StoryDisplayManager displayManager;
    private StoryInterfaceManager interfaceManager;

    #region Singleton
    public static StoryManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //Get references to the story manager components
        scriptManager = StoryScriptManager.Instance;
        displayManager = StoryDisplayManager.Instance;
        interfaceManager = StoryInterfaceManager.Instance;
    }
    #endregion
}
