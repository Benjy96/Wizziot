using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {   

    public override void Execute()
    {
        target = SelectTarget();
        if (target != null)
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

        targetGO = neighbourhood.RetrieveTrackedObject(interestedIn);
        if (targetGO == null) targetGO = neighbourhood.RetrieveTrackedObject(secondaryInterest);
        if (targetGO != null) target = targetGO.transform;

        return target;
    }

    private void Search()
    {
        Debug.Log("Implement Search method");
    }
}