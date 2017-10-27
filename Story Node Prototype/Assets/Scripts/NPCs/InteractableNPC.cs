using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableNPC : MonoBehaviour {

    /**
     * Configuration values are tweaked via the inspector to define our objects.
     * Config values are treated as runtime constants.
     * */
    // ----- SHARED CONFIGURATION OBJECT ----- //
    public NPCStats stats;

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static StoryScriptManager story;

    // ----- STATE VARIABLES ----- //
    [SerializeField] protected string inkPath = "";
    [SerializeField] protected GameObject storyText;

    protected float bobSpeed;
    protected float bobRange;
    protected float pushOffForce;

    // ----- ABSTRACT METHODS ----- //
    public virtual void Interact()
    {
        //Place story text
        //TODO: Implement

        //Set and run story
        story.StoryPosition = inkPath;
        story.DoStory();
    }

    protected abstract void SetExternalFunctions();

    // ----- METHODS ----- //
    protected void Awake()
    {
        //Define the NPC's default stats
        bobSpeed = stats.movementData.bobSpeed;
        bobRange = stats.movementData.bobRange;
        pushOffForce = stats.physicsData.pushOffForce;
    }
    
    protected void Start()
    {
        //Get shared reference to story & player
        if (story == null) story = StoryScriptManager.Instance;
        if (player == null) player = FindObjectOfType<PlayerController>();
        //External functions MUST be set in start -- eliminates "Race" condition - inkStory is set in awake
        SetExternalFunctions();
    }
}
