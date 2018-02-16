/// <summary>
/// All story NPCs - individuals (Bob, Ben) and clones (shopkeeps) alike - (that have dialogue in the Ink Script) inherit from this class
/// </summary>
public abstract class InteractableNPC : Targetable {

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static StoryManager storyManager;

    public string inkPath = "";
    public Mission mission;
    
    // ----- ABSTRACT METHODS ----- //
    protected abstract void RegisterExternalFunctions(); //Bind functions that correlate to ink in here

    // ----- METHODS ----- //
    protected void Start()  //Scripts & variables initialised
    {
        //Get shared reference to the player
        if (player == null) player = PlayerManager.Instance.player.GetComponent<PlayerController>();  //Objects are available when awake is called
        if (storyManager == null) storyManager = StoryManager.Instance;   //Script instances and variables may not have been set in awake - awake is called in random order

        //Set external functions in start - ensures story is loaded up - eliminates race condition
        RegisterExternalFunctions(); 
    }
}