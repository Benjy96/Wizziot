using UnityEngine;
using UnityEngine.AI;

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
            if (hostileToInterests) owner.Influence(Emotion.Anger, .5f * Time.fixedDeltaTime);
            patrolling = false;
            owner.MoveTo(target.position);
        }
        else
        {
            if (Time.time >= patrolResetTime || owner.DestinationReached() || patrolling == false)
            {
                owner.Influence(Emotion.Calm, .2f * Time.fixedDeltaTime);
                patrolling = true;
                owner.MoveToRandomWaypoint();
                patrolResetTime = Time.time + patrolResetDuration;
            }
        }
    }

    public override void ExitState()
    {
        owner.target = target;
        base.ExitState();
    }
}