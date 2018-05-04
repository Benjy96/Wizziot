using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    protected MissionManager missionManager = MissionManager.Instance;

    [HideInInspector] public Mission parent;
    [HideInInspector] public string parentName;
    [HideInInspector] public int missionStage;

    [Header("Displayed in Journal and Log")]
    public string title;
    public string description;

    [Header("Completion Reward - For Multi-Stage, Set in First Stage Only")]
    [Tooltip("Replace x: Resources/Item Prefabs/x")]
    [JsonIgnore] public List<GameObject> missionRewards;

    [Header("Gameplay")]
    public Mission[] additionalMissionStages;
    public Vector3 location;

    [HideInInspector] public string inkName;
    [HideInInspector] public bool completed = false;

    private GameObject waypointObject;
    protected float waypointRadius;
    protected Waypoint waypoint;

    //Insantiate a Mission SO using Resources folder to find asset type
    public Mission CreateMission(string inkName)
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name.Split('(')[0]));
        newMission.parent = newMission;
        newMission.parentName = newMission.parent.name.Split('(')[0];

        //Set stage (parent of chain)
        newMission.missionStage = -1;

        //Link to the Ink script
        newMission.inkName = inkName;

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        return newMission;
    }

    //Insantiate a child mission - inherits the rewards of the parent (first) mission
    public Mission CreateChild()
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name.Split('(')[0]));
        newMission.parent = (Mission)Instantiate(Resources.Load("Mission Objects/" + parentName));

        //Update stage
        newMission.missionStage = missionStage + 1;

        //Link to ink
        newMission.inkName = newMission.parent.inkName;

        //Instantiate & Setup Waypoint Radius
        newMission.waypointObject = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        newMission.waypointRadius = newMission.waypointObject.GetComponent<SphereCollider>().radius;
        newMission.waypoint = newMission.waypointObject.GetComponent<Waypoint>();

        return newMission;
    }

    /// <summary>
    /// Use to destruct mission
    /// </summary>
    public void Complete()
    {
        Destroy(waypointObject);
    }

    public void GrantRewards()
    {
        //Grant rewards
        foreach (GameObject itemPrefab in missionRewards)
        {
            //Make item of prefab
            if (itemPrefab == null) continue;
            GameObject worldItem = Instantiate(itemPrefab, PlayerManager.Instance.player.transform.position, Quaternion.identity);
            Item i = worldItem.GetComponent<Item>();
            if (i != null)
            {
                Debug.Log("Adding to inventory");
                i.AddToInventory();
            }
            else
            {
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
