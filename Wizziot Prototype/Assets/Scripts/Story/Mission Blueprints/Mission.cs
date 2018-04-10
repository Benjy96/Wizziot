using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    protected MissionManager missionManager = MissionManager.Instance;
    public Mission parentMission;

    [Header("Displayed in Journal and Log")]
    public string title;
    public string description;

    [Header("Completion Reward - For Multi-Stage, Set in First Stage Only")]
    [Tooltip("Replace x: Resources/Item Prefabs/x")]
    [JsonIgnore] public List<GameObject> missionRewards;

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
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name.Split('(')[0]));

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        return newMission;
    }

    //Insantiate a child mission - inherits the rewards of the parent (first) mission
    public Mission CreateMission(Mission chainParent, ref List<GameObject> parentRewards)
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name.Split('(')[0]));

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        //Set original mission
        newMission.parentMission = chainParent;
        //Set rewards - deep copy of references
        newMission.missionRewards = new List<GameObject>(parentRewards);

        return newMission;
    }

    /// <summary>
    /// Use to destruct mission
    /// </summary>
    public void CompleteMission()
    {
        Destroy(waypointObject);
        GrantRewards();
    }

    private void GrantRewards()
    {
        //Grant rewards
        foreach (GameObject itemPrefab in missionRewards)
        {
            if (itemPrefab == null) continue;
            //Make item of prefab
            Debug.Log("Instantiating");
            GameObject worldItem = Instantiate(itemPrefab, PlayerManager.Instance.player.transform.position, Quaternion.identity);
            Debug.Assert(worldItem != null);
            Item i = worldItem.GetComponent<Item>();
            Debug.Assert(i != null);
            if (i != null)
            {
                Debug.Log("Adding to inventory");
                i.AddToInventory();
            }
            else
            {
                Debug.Log("Destroying item");
                Destroy(worldItem);
            }
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

    private void CreateWaypoint()
    {
        //Instantiate & Setup Waypoint Radius
        waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        waypointRadius = waypointObject.GetComponent<SphereCollider>().radius;
        waypoint = waypointObject.GetComponent<Waypoint>();
    }
}
