using System;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    //Used by the mission that owns this waypoint to update progress - subscribe update methods
    public Action OnPlayerEnteredWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other entered" + other.name);
        if (other.tag.Equals("Player") || other.name.Equals("Player"))
        {
            if (OnPlayerEnteredWaypoint != null) OnPlayerEnteredWaypoint.Invoke();
        }
    }
}
