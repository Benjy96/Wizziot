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

    public Action onActiveMissionsChanged;   //update missions displayed in UI
    public Action onJournalOpened;

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
        
        //Activate next mission stage
        if(mission.additionalMissionStages.Length > 0)
        {
            //Activate first non-null stage
            foreach (Mission m in mission.additionalMissionStages)
            {
                //If stage is null or has been completed, continue
                if (m == null || completedMissions.Contains(m)) continue;
                else
                {
                    GrantMission(m);
                    break;
                }
            }
        }
        else
        {
            GrantRewards(mission);
        }

        if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
    }

    private void GrantRewards(Mission mission)
    {
        //Grant rewards
        foreach (GameObject item in mission.missionRewards)
        {
            Item i = item.GetComponent<Item>();
            if (i != null)
            {
                i.AddToInventory();
            }
        }
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

        if (onActiveMissionsChanged != null && count > 0) onActiveMissionsChanged.Invoke();

        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i] != null)
            {
                completed[i].CompleteMission();
                FinishMission(completed[i]);
            }
        }
    }

    public void RegisterWaypointReached(Waypoint waypoint)
    {
        Mission[] completed = new Mission[maxMissions];
        int count = 0;
        foreach (Mission mission in activeMissions)
        {
            if (mission == null || mission.GetType() != typeof(Mission)) continue;
            if (mission.location == waypoint.transform.position)
            {
                mission.completed = true;
                completed[count] = mission;
            }
            count++;
        }

        if (onActiveMissionsChanged != null && count > 0) onActiveMissionsChanged.Invoke();

        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i] != null)
            {
                completed[i].CompleteMission();
                FinishMission(completed[i]);
            }
        }
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

        if (onActiveMissionsChanged != null && count > 0) onActiveMissionsChanged.Invoke();

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
