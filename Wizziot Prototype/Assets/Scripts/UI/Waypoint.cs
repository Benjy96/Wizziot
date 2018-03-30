using System;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    //Used by the mission that owns this waypoint to update progress - subscribe update methods
    public Action OnPlayerEnteredWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") || other.name.Equals("Player"))
        {
            MissionManager.Instance.RegisterWaypointReached(this);
        }
    }
}
