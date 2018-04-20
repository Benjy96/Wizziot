using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {

    public float patrolResetDuration = 10f;
    private bool patrolling = false;
    private float patrolResetTime = 0f;

    public override void Execute()
    {
        if(target == null || (target.position - owner.Position).sqrMagnitude > owner.SightRange * owner.SightRange || target == owner.transform)
        {
            target = SelectTarget();
            if(target != null) Debug.Log(name + " is targeting " + target.name);
        }

        if (target != null && owner.CanSeeTarget(target))
        {
            //enemies will attack if not hostile to interest but they are vulnerable - wrong!!!
            //BUT I do like the behaviour -- can make abil comp method to aoe agro around target taking damage
            if (hostileToInterests) owner.Influence(Emotion.Anger, .5f * Time.fixedDeltaTime);
            else if(targetStats.CurrentHealth < targetStats.maxHealth)  
            {
                owner.Influence(Emotion.Anger, .25f * Time.fixedDeltaTime);
                if(targetStats.CurrentHealth < targetStats.maxHealth / 3)
                {
                    owner.Influence(Emotion.Anger, 1f * Time.fixedDeltaTime);
                }
            }
            patrolling = false;
            owner.MoveTo(target.position);
        }
        else
        {
            if (Time.time >= patrolResetTime || owner.DestinationReached() || patrolling == false)
            {
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