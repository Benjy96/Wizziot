/// <summary>
/// All story NPCs - individuals (Bob, Ben) and clones (shopkeeps) alike - (that have dialogue in the Ink Script) inherit from this class
/// </summary>
public abstract class InteractableNPC : Targetable {

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static StoryManager storyManager;
    protected static new TargetType targetType = TargetType.Story;

    public string inkPath = "";
    
    // ----- ABSTRACT METHODS ----- //
    protected abstract void RegisterExternalFunctions(); //Bind functions that correlate to ink in here

    // ----- METHODS ----- //
    protected void Awake()  //Objects initialised in scene
    {
        player = FindObjectOfType<PlayerController>();  //Objects are available when awake is called
    }

    protected void Start()  //Scripts & variables initialised
    {
        //Get shared reference to the player
        storyManager = StoryManager.Instance;   //Script instances and variables may not have been set in awake - awake is called in random order

        RegisterExternalFunctions(); //Set external functions in start - ensures story is loaded up - eliminates race condition
    }
}