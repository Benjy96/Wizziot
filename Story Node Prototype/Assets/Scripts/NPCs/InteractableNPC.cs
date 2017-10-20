using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    /**
     * Configuration values are tweaked via the inspector to define our objects.
     * */
    // ----- CONFIGURATION OBJECT ----- //
    public NPCStats stats;

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static Script story;

    // ----- STATE VARIABLES ----- //
    [SerializeField] protected string inkPath = "";

    protected Vector3 InteractingNPC
    {
        get
        {
            return player.TargetPos;
        }
    }

    protected float bobSpeed;
    protected float bobRange;
    protected float pushOffForce;

    // ----- ABSTRACT METHODS ----- //
    public abstract void Interact();

    protected abstract void SetExternalFunctions();

    // ----- METHODS ----- //
    protected void Awake()
    {
        //Get shared reference to story & player
        if (story == null) story = FindObjectOfType<Script>();
        if (player == null) player = FindObjectOfType<PlayerController>();

        //Define the NPC's default stats
        Debug.Assert(stats != null);
        bobSpeed = stats.movementData.bobSpeed;
        bobRange = stats.movementData.bobRange;
        pushOffForce = stats.physicsData.pushOffForce;
    }
    
    protected void Start()
    {
        //External functions MUST be set in start -- eliminates "Race" condition - inkStory is set in awake
        SetExternalFunctions();
    }
}
