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
    protected static StoryManager storyManager;

    // ----- STATE VARIABLES ----- //
    public string inkPath = "";

    protected float bobSpeed;
    protected float bobRange;
    protected float pushOffForce;
    
    // ----- ABSTRACT METHODS ----- //
    protected abstract void SetExternalFunctions(); //Bind functions that correlate to ink in here

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
        //Get shared reference to the player
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (storyManager == null) storyManager = StoryManager.Instance;

        SetExternalFunctions(); //Set external functions in start - ensures story is loaded up - eliminates race condition
    }
}