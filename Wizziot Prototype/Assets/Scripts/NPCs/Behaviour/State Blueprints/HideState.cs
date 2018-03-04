using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HideState", menuName = "States/Hide")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class HideState : State {

	// Use this for initialization
	void Start () {
        neighbourhoodTracker.TrackObstacles();
	}

    /**
        1.) Seeker.pos - object.pos = direction through object
        2.) Normalize for direction vector
        3.) Set magnitude to a factor of object radius (so hide position is not inside object)
        4.) Move agent to calculated hide position.

        Example Modifications:
1 - Hide only if in view. 2 - Favour rear of target. 3 - Hide only if under threat.
    */
    void Update () {
        Transform closest = null;
        Vector3 distance = Vector3.zero;

        foreach (Transform x in neighbourhoodTracker.obstacles)
        {
            if (closest == null)
            {
                distance = x.position - owner.Position;
                closest = x;
            }
            else if(distance.sqrMagnitude > (x.position - owner.Position).sqrMagnitude)
            {
                closest = x;
            }
        }

        Vector3 dir = (influencer.transform.position - closest.position).normalized;
        float obstacleSizeX = closest.transform.localScale.x;
        float obstacleSizeZ = closest.transform.localScale.z;
        float biggest = (obstacleSizeX > obstacleSizeZ) ? obstacleSizeX : obstacleSizeZ;

        Vector3 adjustedForObstacleSizePos = dir * biggest;

        owner.MoveTo(adjustedForObstacleSizePos);
	}
}
