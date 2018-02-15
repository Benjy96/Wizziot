using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add UI Representation
public class MissionManager : MonoBehaviour {

    private static MissionManager _MissionManager;
    public static MissionManager Instance { get { return _MissionManager; } }

    public List<Mission> activeMissions;
    public int maxMissions = 3;

    public Action onMissionCompleted; 

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
    }

    //TODO: Watch brackeys combat/enemies and events to see if events would be more applicable than inventory style system
    //Who calls this? In enemy Die() method? Player Controller?
    public void RegisterKill(Enemy enemy)
    {
        List<Mission> missions = activeMissions.FindAll(x => x is Mission);

        foreach (KillMission killMission in missions)
        {
            killMission.UpdateMission(enemy);
        }
    }

    public void RegisterWaypointReached()
    {

    }

    public void RegisterItemFound()
    {

    }
}
