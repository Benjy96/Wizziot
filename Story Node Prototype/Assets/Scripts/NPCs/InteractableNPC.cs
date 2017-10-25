using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    /**
     * Configuration values are tweaked via the inspector to define our objects.
     * Config values are treated as runtime constants.
     * */
    // ----- SHARED CONFIGURATION OBJECT ----- //
    public NPCStats stats;

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected Script story;

    // ----- STATE VARIABLES ----- //
    [SerializeField] protected string inkPath = "";
    [SerializeField] protected GameObject storyText;

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
    public virtual void Interact()
    {
        /* BENEATH IS ASSIGNED BY REFERENCE - GAMEOBJECT IS A CLASS - WE AREN'T CREATING NEW OBJECTS FOR EACH CHILD */
        storyText = story.displayStoryAsset;
        storyText.transform.position = transform.position;
        storyText.SetActive(true);
    }

    protected abstract void SetExternalFunctions();

    // ----- METHODS ----- //
    protected void Awake()
    {
        //Get shared reference to story & player
        if (story == null) story = FindObjectOfType<Script>();
        if (player == null) player = FindObjectOfType<PlayerController>();

        //Define the NPC's default stats
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
