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
        if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
    }

    public void RegisterKill()
    {
        Mission[] completed = new Mission[maxMissions];
        int count = 0;
        foreach (Mission mission in activeMissions)
        {
            if (mission == null || mission.GetType() != typeof(KillMission)) continue;
            mission.UpdateMission(PlayerManager.Instance.playerControls.Target);
            if (mission.completed) completed[count] = mission;
            count++;
        }

        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i] != null)
            {
                completed[i].CompleteMission();
                FinishMission(completed[i]);
            }
        }
    }

    public void RegisterWaypointReached(Vector3 waypoint)
    {
        Mission matchingMission = activeMissions.Find(x => x.location == waypoint);
    }

    public void RegisterItemFound(Item item)
    {
        Mission[] completed = new Mission[maxMissions];
        int count = 0;
        foreach (Mission mission in activeMissions)
        {
            if (mission == null || mission.GetType() != typeof(CollectMission)) continue;
            mission.UpdateMission(item);
            if (mission.completed) completed[count] = mission;
            count++;
        }

        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i] != null)
            {
                completed[i].CompleteMission();
                FinishMission(completed[i]);
            }
        }
    }
}
