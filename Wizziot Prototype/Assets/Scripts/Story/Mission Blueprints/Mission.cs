using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    MissionManager missionManager = MissionManager.Instance;

    [Header("Displayed in Journal and Log")]
    public string title;
    public string description;

    [Header("Completion Reward - For Multi-Stage, Set in First Stage Only")]
    public GameObject missionReward1;       //CHANGE TO PREFAB NAME STRING FOR SERIALISATION
    public GameObject missionReward2;
    public GameObject missionReward3;
    [HideInInspector] public List<GameObject> missionRewards;

    [Header("Gameplay")]
    public Mission[] additionalMissionStages;
    public Vector3 location;

    private GameObject waypointObject;
    protected float waypointRadius;
    protected Waypoint waypoint;

    [HideInInspector] public bool completed = false;

    private void Awake()
    {
        missionRewards = new List<GameObject>(3)
        {
            missionReward1,
            missionReward2,
            missionReward3
        };
    }

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

    //Insantiate a child mission - inherits the rewards of the parent (first) mission
    public Mission CreateMission(List<GameObject> firstStageRewards)
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name));

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        //Set rewards
        newMission.missionRewards = firstStageRewards;

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
