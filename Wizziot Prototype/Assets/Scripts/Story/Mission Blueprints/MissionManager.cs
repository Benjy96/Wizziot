using System;
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
        //DontDestroyOnLoad(gameObject);

        activeMissions = new List<Mission>(maxMissions);
        completedMissions = new List<Mission>(maxMissions);
    }

    /// <summary>
    /// Grant a mission or first stage of chain mission
    /// </summary>
    public void GrantMission(Mission mission, string inkMission)
    {
        Mission grantedMission = mission.CreateMission(inkMission);

        if (!activeMissions.Contains(grantedMission) && activeMissions.Count < maxMissions)
        {
            activeMissions.Add(grantedMission);

            //Update UI (Mission Log)
            if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
        }
    }

    /// <summary>
    /// Grant a new part of a multi-stage mission, passing through the parent's rewards
    /// </summary>
    public void GrantChildMission(Mission mission)
    {
        Mission grantedMission;
        //If the first of a chain (no parent), set child's parent reference as this, else carry previous parent

        if (mission.parent.additionalMissionStages == null)
        {
            grantedMission = mission.CreateMission(mission.inkName);
        }
        else
        {
            grantedMission = mission.CreateChild();
        }

        if (!activeMissions.Contains(grantedMission) && activeMissions.Count < maxMissions)
        {
            activeMissions.Add(grantedMission);

            //ACTIVATE UI
            if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
        }
    }

    /// <summary>
    /// Use to register a mission as complete
    /// </summary>
    public void FinishMission(Mission mission)
    {
        activeMissions.Remove(mission);
        completedMissions.Add(mission);
        
        //Activate next mission stage (As parent)
        if(mission.additionalMissionStages.Length > mission.missionStage)
        {
            Debug.Log("here");
            GrantChildMission(mission);
        }
        else
        {
            Debug.Log("granting rewards");
            //If no stages left, grant the rewards
            mission.GrantRewards();
            StoryManager.Instance.CompleteInkMission(mission.inkName);
        }

        mission.Complete(); //"Destructor" - Removes waypoints from world

        if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
    }

    public void RegisterKill(Enemy e)
    {
        Mission[] completed = new Mission[maxMissions];
        int count = 0;
        foreach (Mission mission in activeMissions)
        {
            if (mission == null || mission.GetType() != typeof(KillMission)) continue;
            mission.UpdateMission(e);
            if (mission.completed) completed[count] = mission;
            count++;
        }

        if (onActiveMissionsChanged != null && count > 0) onActiveMissionsChanged.Invoke();

        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i] != null)
            {
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
                FinishMission(completed[i]);
            }
        }
    }
}
