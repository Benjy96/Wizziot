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
    protected static StoryManager story;

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
        //TODO: Decouple this UI code (add to a component)
        //Get space above head for the story text
        float objectHeight = transform.localScale.y;
        Vector3 textPos = new Vector3(transform.position.x, transform.position.y + objectHeight, transform.position.z);

        /* BENEATH IS ASSIGNED BY REFERENCE - GAMEOBJECT IS A CLASS - WE AREN'T CREATING NEW OBJECTS FOR EACH CHILD */
        storyText = story.displayStoryObject;
        storyText.transform.position = textPos;
        storyText.transform.SetParent(gameObject.transform);
        //Enable canvas and text
        storyText.SetActive(true);
        storyText.transform.GetChild(0).gameObject.SetActive(true);

        //Set and run story
        story.StoryPosition = inkPath;
        story.DoStory();
    }

    protected abstract void SetExternalFunctions();

    // ----- METHODS ----- //
    protected void Awake()
    {
        //Get shared reference to story & player
        if (story == null) story = StoryManager.Instance;
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
