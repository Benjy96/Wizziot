using UnityEngine;

[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    public string title;

    MissionManager missionManager = MissionManager.Instance;

    public Mission[] additionalMissionStages;
    public Vector3 location;

    public bool completed = false;

    private GameObject waypointObject;
    protected float waypointRadius;
    protected Waypoint waypoint;

    //Insantiate a Mission SO using Resources folder to find asset type
    public Mission CreateMission()
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name));

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();
        
        //Event Subscription - Call Update Mission (No params) on Player enter trigger event
        newMission.waypoint.OnPlayerEnteredWaypoint += UpdateMission;

        return newMission;
    }

    public void CompleteMission()
    {
        if (missionManager.activeMissions.Contains(this))
        {
            //Event Removal
            waypoint.OnPlayerEnteredWaypoint -= UpdateMission;
            //Remove waypoint
            Destroy(waypointObject);
        }
        else
        {
            throw new System.Exception("Mission manager is not storing this mission!: " + name);
        }
    }

    //Called by an event "OnPlayerEnteredWaypoint" when player enters waypoint
    public virtual void UpdateMission()
    {
        if(additionalMissionStages.Length > 0)
        {
            return;
        }
        else
        {
            completed = true;
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
