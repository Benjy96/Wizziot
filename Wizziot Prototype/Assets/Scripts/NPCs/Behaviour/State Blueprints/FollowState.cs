using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {   

    public GameObject secondaryTarget;  

    protected override State EnterState(Enemy owner)
    {   
        neighbourhood.RegisterInterest(secondaryTarget);
        return base.EnterState(owner);
    }

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
        neighbourhood.RemoveInterest(secondaryTarget);
        owner.target = target;
        base.ExitState();
    }

    protected override Transform SelectTarget()
    {
        GameObject targetGO = null;
        Transform target = null;

        targetGO = neighbourhood.RetrieveTrackedObject(interestedIn);
        if (targetGO == null) targetGO = neighbourhood.RetrieveTrackedObject(secondaryTarget);
        if (targetGO != null) target = targetGO.transform;

        return target;
    }

    private void Search()
    {
        Debug.Log("Implement Search method");
    }
}