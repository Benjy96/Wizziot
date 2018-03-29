using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour {

    private static MissionManager _MissionManager;
    public static MissionManager Instance { get { return _MissionManager; } }

    public MissionUIManager missionUI;
    public GameObject waypointPrefab;

    public List<Mission> activeMissions;
    private List<Mission> completedMissions;
    public int maxMissions = 3;

    public Action onActiveMissionsChanged;   //update UI etc

    private void Awake()
    {
        if (_MissionManager == null)
        {
            _MissionManager = this;
        }
        else if (_MissionManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        activeMissions = new List<Mission>(maxMissions);
        completedMissions = new List<Mission>(maxMissions);
    }

    public void GrantMission(Mission mission)
    {
        Mission grantedMission = mission.CreateMission();

        //TODO: Change to dict -> if contains(mission.name) - using SOs and need to differentiate missions
        if (!activeMissions.Contains(grantedMission) && activeMissions.Count < maxMissions)
        {
            activeMissions.Add(grantedMission);

            //ACTIVATE UI
            if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
        }
    }

    public void FinishMission(Mission mission)
    {
        activeMissions.Remove(mission);
        completedMissions.Add(mission);
    }

    public void RegisterKill()
    {
        foreach (Mission mission in activeMissions)
        {
            if (mission == null || mission.GetType() != typeof(KillMission)) continue;
            Debug.Log("Updating mission");
            mission.UpdateMission(PlayerManager.Instance.playerControls.Target);
        }
    }

    public void RegisterWaypointReached(Vector3 waypoint)
    {
        Mission matchingMission = activeMissions.Find(x => x.location == waypoint);
    }

    public void RegisterItemFound(Item item)
    {
        foreach (CollectMission mission in activeMissions)
        {
            mission.UpdateMission(item);
        }
    }
}
