using UnityEngine;
/// <summary>
/// All story NPCs - individuals (Bob, Ben) and clones (shopkeeps) alike - (that have dialogue in the Ink Script) inherit from this class
/// </summary>
public class InteractableNPC : Targetable {

    // ----- CONFIGURATION VARIABLES ----- //
    protected static PlayerController player;
    protected static StoryManager storyManager;

    [Header("Section of ink script dialogue for NPC")]
    public string inkConversationPath = "";
    [Header("Element numbers correspond to missions array")]
    public string[] inkMissionNames;
    [Tooltip("Elements correspond to inkMissionNames array")]
    public Mission[] missions;

    // ----- VIRTUAL METHODS ----- //
    //Missions are linked explicity
    //Example: Interacble NPCs are characters: Bob.cs, Ben.cs, Sphere.cs
    //Let's say each has 3 quests, we need to define these as externals in Ink if we want it granted through story
    //We then Add those functions as externals in their scripts, so 3 missions means 3 functions to bind to ink
    /// <summary>
    /// Override this method to register external functions. 
    /// By default registers and links each element of missions/missionNames. 
    /// For example, the first element of each array links an ink function (granting the mission) to the mission object.
    /// </summary>
    protected virtual void RegisterExternalMissions()
    {
        if ((inkMissionNames != null && missions != null) && inkMissionNames.Length == missions.Length)
        {
            int counter = 0;
            foreach (Mission m in missions)
            {
                if (m == null) continue;
                string inkMission = inkMissionNames[counter];
                storyManager.BindExternalFunction(inkMissionNames[counter], () => GrantMission(m, inkMission));
                counter++;
            }
        }
    }

    /// <summary>
    /// Register external functions in here, non-mission related, like applying a force. Example:
    /// </summary>
    protected virtual void RegisterExternalFunctions()
    {
        //storyManager.BindExternalFunction("CubePushOff", CubePushOff);
    }

    // ----- METHODS ----- //
    protected void Start()  //Scripts & variables initialised
    {
        targetType = TargetType.Story;

        //Get shared reference to the player
        if (player == null) player = PlayerManager.Instance.player.GetComponent<PlayerController>();  //Objects are available when awake is called
        if (storyManager == null) storyManager = StoryManager.Instance;   //Script instances and variables may not have been set in awake - awake is called in random order

        //Set external functions in start - ensures story is loaded up - eliminates race condition
        RegisterExternalFunctions();
        RegisterExternalMissions();
    }

    /// <summary>
    /// Can override this method to grant more missions
    /// </summary>
    protected virtual void GrantMission(Mission mission, string inkMissionName)
    {
        MissionManager.Instance.GrantMission(mission, inkMissionName);
    }
}