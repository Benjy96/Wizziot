using UnityEngine;

[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    MissionManager missionManager = MissionManager.Instance;

    [Header("Displayed in Journal and Log")]
    public string title;
    public string description;

    [Header("Completion Reward")]
    public GameObject[] missionRewards;

    [Header("Gameplay")]
    public Mission[] additionalMissionStages;
    public Vector3 location;

    private GameObject waypointObject;
    protected float waypointRadius;
    protected Waypoint waypoint;

    [HideInInspector] public bool completed = false;

    //Insantiate a Mission SO using Resources folder to find asset type
    public Mission CreateMission()
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name));

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        return newMission;
    }

    public void CompleteMission()
    {
        if (missionManager.activeMissions.Contains(this))
        {
            //Remove waypoint
            Destroy(waypointObject);
        }
        else
        {
            throw new System.Exception("Mission manager is not storing this mission!: " + name);
        }
    }

    /// <summary>
    /// Determine how the target is handled with regards to the mission type
    /// </summary>
    /// <param name="target">For example, a killed enemy or picked up item</param>
    public virtual void UpdateMission(Targetable target)
    {
        Debug.Log("Update the mission progress");
    }
}
