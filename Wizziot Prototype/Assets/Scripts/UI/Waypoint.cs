using System;
using UnityEngine;

//Registers waypoint reached with mission manager. 
//Dynamic size, i.e., the player mission updates (enters) at dist 8 (from Update() tracking), but leaves at dist 16 (size of collider)
public class Waypoint : MonoBehaviour {

    bool trackPlayerDist = false;
    Collider player;
    SphereCollider wpCollider;

    //Used by the mission that owns this waypoint to update progress - subscribe update methods
    public Action OnPlayerEnteredWaypoint;

    private void Awake()
    {
        wpCollider = GetComponent<SphereCollider>();
    }

    //Enter at radius 16 & start tracking the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") || other.name.Equals("Player"))
        {
            player = other;
            trackPlayerDist = true;
        }
    }

    //Will leave at radius 16
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") || other.name.Equals("Player"))
        {
            player = null;
            trackPlayerDist = false;
        }
    }

    //If tracking place distance, update the mission when we have reached the waypoint
    private void Update()
    {
        if (trackPlayerDist)
        {
            //If player is a third of the radius distance to the center of the waypoint, register we have reached it
            if((player.transform.position - transform.position).sqrMagnitude <= (wpCollider.radius * wpCollider.radius) / 4)
            {
                MissionManager.Instance.RegisterWaypointReached(this);
                trackPlayerDist = false;
            }
        }
    }
}
