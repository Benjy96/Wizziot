﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All story NPCs (that have dialogue in the Ink Script) inherit from this class
/// </summary>
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
    protected abstract void RegisterExternalFunctions(); //Bind functions that correlate to ink in here

    // ----- METHODS ----- //
    protected void Awake()  //Objects initialised in scene
    {
        player = FindObjectOfType<PlayerController>();  //Objects are available when awake is called
        
        //Define the NPC's default stats
        bobSpeed = stats.movementData.bobSpeed;
        bobRange = stats.movementData.bobRange;
        pushOffForce = stats.physicsData.pushOffForce;
    }

    protected void Start()  //Scripts & variables initialised
    {
        //Get shared reference to the player
        storyManager = StoryManager.Instance;   //Script instances and variables may not have been set in awake - awake is called in random order

        RegisterExternalFunctions(); //Set external functions in start - ensures story is loaded up - eliminates race condition
    }
}