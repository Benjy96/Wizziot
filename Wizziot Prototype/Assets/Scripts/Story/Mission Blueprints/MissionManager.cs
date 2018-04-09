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

    /// <summary>
    /// Grant a new part of a multi-stage mission, passing through the parent's rewards
    /// </summary>
    public void GrantChildMission(Mission mission)
    {
        Mission grantedMission;
        //If the first of a chain, set child's parent reference as this, else carry previous parent
        if (mission.parentMission == null)
        {
            grantedMission = mission.CreateMission(mission);
        }
        else
        {
            grantedMission = mission.CreateMission(mission.parentMission); 
        }

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
        
        //Activate next mission stage (From parent)
        if(mission.additionalMissionStages.Length > 0)
        {
            //Activate first non-null stage
            foreach (Mission m in mission.additionalMissionStages)
            {
                //If stage is null or has been completed, continue
                if (m == null || completedMissions.Contains(m)) continue;
                else
                {
                    //Grant the child mission, and pass through the parent rewards
                    if (m.missionRewards.Count != 0) GrantChildMission(m);
                    else GrantMission(m);
                    break;
                }
            }
        }
        else if (mission.parentMission.additionalMissionStages.Length > 0)  //(From child)
        {
            //Activate first non-null stage
            foreach (Mission m in mission.parentMission.additionalMissionStages)
            {
                //If stage is null or has been completed, continue
                if (m == null || completedMissions.Contains(m)) continue;
                else
                {
                    //Grant the child mission, and pass through the parent rewards
                    if (m.missionRewards.Count != 0) GrantChildMission(m);
                    else GrantMission(m);
                    break;
                }
            }
        }
        else
        {
            //If no stages left, grant the rewards
            GrantRewards(mission);
        }

        if (onActiveMissionsChanged != null) onActiveMissionsChanged.Invoke();
    }

    private void GrantRewards(Mission mission)
    {
        //Grant rewards
        foreach (GameObject itemPrefab in mission.missionRewards)
        {
            //Make item of prefab
            GameObject worldItem = Instantiate(itemPrefab, PlayerManager.Instance.player.transform.position, Quaternion.identity);
            Item i = worldItem.GetComponent<Item>();
            if (i != null)
            {
                i.AddToInventory();
            }
            else
            {
                Destroy(worldItem);
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
