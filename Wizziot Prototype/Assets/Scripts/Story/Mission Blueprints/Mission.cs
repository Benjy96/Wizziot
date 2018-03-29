using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    MissionManager missionManager = MissionManager.Instance;

    public Mission[] additionalMissionStages;
    public Vector3 location;
    public float locationRadius;

    protected bool completed = false;

    private GameObject waypoint;

    public Mission CreateMission()
    {
        Mission newMission = (Mission)Instantiate(Resources.Load("Mission Objects/" + name));
        newMission.waypoint = Instantiate(MissionManager.Instance.waypointPrefab, location, Quaternion.Euler(-90f, 0f, 0f));
        return newMission;
    }

    public void CompleteMission()
    {
        if (missionManager.activeMissions.Contains(this))
        {
            Debug.Log("Destroying wp");
            Destroy(waypoint);
            completed = true;
            MissionManager.Instance.FinishMission(this);
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
