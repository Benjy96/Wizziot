using UnityEngine;

[CreateAssetMenu(fileName = "FollowState", menuName = "States/Follow")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FollowState : State {   

    public GameObject primaryTarget;
    public GameObject secondaryTarget;  

    private NeighbourhoodTracker neighbourhood;
    private EnemySpawnPoint spawn;
    private Transform target;

    protected override State EnterState(Enemy owner)
    {
        this.owner = owner;
        neighbourhood = owner.GetComponent<NeighbourhoodTracker>();
        spawn = owner.Spawn;

        neighbourhood.RegisterInterest(primaryTarget);
        neighbourhood.RegisterInterest(secondaryTarget);

        return this;
    }

    public override void Execute()
    {
        target = SelectTarget();
        if (target != null)
        {
            MoveTo(target.position);
        }
        else
        {
            Patrol();
        }
    }

    private Transform SelectTarget()
    {
        GameObject targetGO = null;
        Transform target = null;

        targetGO = neighbourhood.RetrieveTrackedObject(primaryTarget);
        if (targetGO == null) targetGO = neighbourhood.RetrieveTrackedObject(secondaryTarget);
        if (targetGO != null) target = targetGO.transform;

        return target;
    }

    protected void MoveTo(Vector3 target)
    {
        owner.navAgent.destination = target;
        FaceTarget(target);
    }

    protected void Patrol()
    {
        Debug.Log("Patrolling");
        if (TargetReached())
        {
            Debug.Log("Picking new co-ordinates");
            int randomIndex = Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count);
            MoveTo(owner.Spawn.spawnAreaWaypoints[randomIndex]);
            //TODO: Add not reached destination timer
        }
    }

    protected bool TargetReached()
    {
        return owner.navAgent.remainingDistance <= (owner.navAgent.stoppingDistance + (owner.navAgent.stoppingDistance / 2));
    }


    protected void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - owner.Position;
        direction = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.fixedDeltaTime * 10f);
    }
}