using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {   

    public override void Execute()
    {
        target = SelectTarget();
        if (target != null && owner.CanSeeTarget(target))
        {
            owner.MoveTo(target.position);
        }
        else
        {
            Search();
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

    private void Search()
    {
        Vector3 patrolPos = new Vector3(Mathf.Cos(Time.time), 0f, Mathf.Sin(Time.time)) * 10;
        owner.MoveTo(patrolPos);
    }
}