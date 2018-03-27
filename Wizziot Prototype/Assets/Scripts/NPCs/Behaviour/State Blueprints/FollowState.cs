using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {

    public float patrolResetDuration = 10f;
    private bool patrolling = false;
    private float patrolResetTime = 0f;

    public override void Execute()
    {
        target = SelectTarget();
        if (target != null && owner.CanSeeTarget(target))
        {
            if (hostileToInterests) owner.Influence(Emotion.Anger, .5f);
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

    private void MoveToRandomWaypoint()
    {
        //Get random point, based upon spawner waypoints
        Vector3 patrolPos = owner.Spawn.spawnAreaWaypoints[Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count)];

        //Prevent waypoint at current location being set as new target
        if (patrolPos == owner.Position)
        {
            MoveToRandomWaypoint();
            return;
        }

        owner.MoveTo(patrolPos);
    }
}