using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {

    //TODO: Fix - Not seen player - patrol as first action

    public float patrolResetDuration = 10f;
    private bool patrolling = false;
    private float patrolResetTime = 0f;

    public override void Execute()
    {
        target = SelectTarget();
        if (target != null && owner.CanSeeTarget(target))
        {
            patrolling = false;
            owner.MoveTo(target.position);
        }
        else
        {
            if (Time.time >= patrolResetTime || owner.DestinationReached() || patrolling == false)
            {
                patrolling = true;
                MoveToRandomWaypoint();
                patrolResetTime = Time.time + patrolResetDuration;
            }
        }
    }

    public override void ExitState()
    {
        owner.target = target;
        base.ExitState();
    }

    protected override Transform SelectTarget()
    {
        GameObject targetGO = null;
        Transform target = null;

        targetGO = neighbourhoodTracker.RetrieveTrackedObject(interestedIn);
        if (targetGO == null) targetGO = neighbourhoodTracker.RetrieveTrackedObject(secondaryInterest);
        if (targetGO != null) target = targetGO.transform;

        return target;
    }

    private void MoveToRandomWaypoint()
    {
        //Get random point, based upon spawner waypoints
        Vector3 pointOrigin = owner.Spawn.spawnAreaWaypoints[Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count)];
        Vector3 patrolPos = new Vector3(pointOrigin.x, 0f, pointOrigin.z);

        //Prevent waypoint at current location being set as new target
        if (patrolPos == owner.Position)
        {
            MoveToRandomWaypoint();
            return;
        }

        owner.MoveTo(patrolPos);
    }
}