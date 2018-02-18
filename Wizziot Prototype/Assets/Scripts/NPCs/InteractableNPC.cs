/// <summary>
/// All story NPCs - individuals (Bob, Ben) and clones (shopkeeps) alike - (that have dialogue in the Ink Script) inherit from this class
/// </summary>
public abstract class InteractableNPC : Targetable {

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static StoryManager storyManager;

    public string inkPath = "";
    public Mission mission;

    // ----- VIRTUAL METHODS ----- //
    //Ok - missions are going to have to be done explicity (link ink - unity)
    //Example: Interacble NPCs are characters: Bob.cs, Ben.cs, Sphere.cs
    //Let's say each has 3 quests, we need to define these as externals in Ink if we want it granted through story
    //We then Add those functions as externals in their scripts, so 3 missions means 3 functions to bind to ink
    //Not too flexible, but it is flexible enough in that we can create Mission objects and modify the variables very easily.
    protected abstract void RegisterExternalFunctions();

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