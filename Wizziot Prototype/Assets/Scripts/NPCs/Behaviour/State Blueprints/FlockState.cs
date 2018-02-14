﻿using UnityEngine;

[CreateAssetMenu(fileName = "FlockState", menuName = "States/Flock")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FlockState : State {   //TODO: make an "anti-flock" state based upon emotional goal (e.g. make SO for scared goal, this one for angry goal)

    public GameObject primaryTarget;
    public GameObject secondaryTarget;  
    //Notes: Area detection || Spawner: Keep list of references to all "Objects" within collider radius which can be a target
    //Or use above variable as Prefab for the target TYPE - then do FindObjectOfType<secondaryTarget>() within RADIUS (collider)
    //public GameObject[] potentialTargets; //if(checkArea(potentialTargets[i]) secondaryTarget = checkArea(potentialTargets[i]);

    public float velocityMatchingWeight = 0.25f;
    public float flockCenteringWeight = 0.2f;
    public float attractionWeight = 2f;
    public float repulsionWeight = 2f;

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
            Debug.Log("Patrolling");
            if(owner.navAgent.remainingDistance <= owner.navAgent.stoppingDistance)
            {
                Debug.Log("Picking new co-ordinates");
                int randomIndex = Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count);
                MoveTo(owner.Spawn.spawnAreaWaypoints[randomIndex]);
                //TODO: Add not reached destination timer
            }
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

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - owner.Position;
        direction = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.fixedDeltaTime * 10f);
    }

    private void MoveTo(Vector3 target)
    {
        owner.navAgent.destination = target;
        FaceTarget(target);
    }
}