using UnityEngine;

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

    protected override State EnterState(Enemy owner)
    {
        this.owner = owner;
        neighbourhood = owner.GetComponent<NeighbourhoodTracker>();
        spawn = owner.Spawn;

        neighbourhood.RegisterInterest(primaryTarget);
        neighbourhood.RegisterInterest(secondaryTarget);

        return this;
    }

    //TODO: Add Behaviour for when player out of range
    public override void Execute()
    {
        Transform target = SetTarget();
        if (target != null)
        {
            Debug.Log("Targeting: " + target.name);
            //Get current velocity - going to be modified
            Vector3 vel = owner.navAgent.destination;

            //Velocity Matching - match velocity of neighbors
            Vector3 velAlign = neighbourhood.AvgVel;

            //Flock centering - move towards center of local neighbors
            Vector3 velCenter = neighbourhood.AvgPos;
            if (velCenter != Vector3.zero)
            {
                velCenter -= owner.Position;    //look to center
            }

            //Attraction
            Vector3 attractDelta = target.position - owner.Position;    //Agent to attractor vector

            //Attract if target is within targeting distance
            bool attracted = (owner.navAgent.stoppingDistance < attractDelta.magnitude);   //If distance less than max target distance, NPC attracted to target

            //Apply ALL velocities - the weighting will help influence how much of an impact "influence" each has. Each vector has an affect since vel is assigned and used each time
            float fdt = Time.fixedDeltaTime;

            if (velAlign != Vector3.zero)   //If we need to align
            {
                vel = Vector3.Lerp(vel, velAlign, velocityMatchingWeight * fdt);
            }

            if (velCenter != Vector3.zero)  //If we need to center
            {
                vel = Vector3.Lerp(vel, velCenter, flockCenteringWeight * fdt);
            }

            if (attractDelta != Vector3.zero) //If we need to go towards target
            {
                if (attracted)  //if distance from attractor is big enough
                {
                    vel = Vector3.Lerp(vel, attractDelta, attractionWeight * fdt);
                }
            }
            owner.navAgent.SetDestination(vel);
            FaceTarget(target);
        }
    }

    private Transform SetTarget()
    {
        GameObject targetGO = null;
        Transform target = null;

        targetGO = neighbourhood.RetrieveTrackedObject(primaryTarget);
        if (targetGO == null) targetGO = neighbourhood.RetrieveTrackedObject(secondaryTarget);
        if (targetGO != null) target = targetGO.transform;

        return target;
    }

    private void FaceTarget(Transform target)
    {
        Vector3 direction = target.position - owner.Position;
        direction = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
    }
}