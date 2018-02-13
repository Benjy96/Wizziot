using UnityEngine;

[CreateAssetMenu(fileName = "Flock State", menuName = "States/Flock")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FlockState : State {   //TODO: make an "anti-flock" state based upon emotional goal (e.g. make SO for scared goal, this one for angry goal)

    //TODO: May need to remove navagent
    public float collisionAvoidanceWeight = 2f;
    public float velocityMatchingWeight = 0.25f;
    public float flockCenteringWeight = 0.2f;
    public float attractionWeight = 10f;
    public float repulsionWeight = 2f;

    private NeighbourhoodTracker neighbourhood;
    private EnemySpawnPoint spawn;

    protected override State EnterState(Enemy owner)
    {
        this.owner = owner;
        neighbourhood = owner.GetComponent<NeighbourhoodTracker>();
        spawn = owner.Spawn;
        return this;
    }

    //TODO: Add Behaviour for when player out of range
    //TODO: Fix up vector calculations (magnitude) for using a nav mesh v just vectors from Boids
    public override void Execute()
    {
        Debug.Log(attractionWeight);
        Transform target = owner.target;
        Vector3 vel = owner.navAgent.destination;

        //Collision avoidance - avoid neighbours that are too close
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooClosePos = neighbourhood.AvgTooClosePos;
        if (tooClosePos != Vector3.zero)
        {
            velAvoid = owner.Position - tooClosePos;   //turn away
        }

        //Velocity Matching - match velocity of neighbors
        Vector3 velAlign = neighbourhood.AvgVel;

        //Flock centering - move towards center of local neighbors
        Vector3 velCenter = neighbourhood.AvgPos;
        if (velCenter != Vector3.zero)
        {
            velCenter -= owner.Position;    //look to center
        }

        //Attraction
        Vector3 attractDelta = target.transform.position - owner.Position;    //Agent to attractor vector
        //TODO: Use emotion for attraction? Could create a reverse flock for running away when scared
        //Attract if target is within targeting distance
        //TODO: Create a stopping distance
        bool attracted = (attractDelta.sqrMagnitude < owner.stats.sqrMaxTargetDistance && owner.navAgent.stoppingDistance < attractDelta.magnitude);   //If distance less than max target distance, NPC attracted to target

        //Apply ALL velocities - the weighting will help influence how much of an impact "influence" each has. Each vector has an affect since vel is assigned and used each time
        float fdt = Time.fixedDeltaTime;
       // if (velAvoid != Vector3.zero)    //if we need to do avoidance
      //  {
        //    vel = Vector3.Lerp(vel, velAvoid, collisionAvoidanceWeight * fdt);
       // }
       // else //else do normal movements
       // {
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
                else
                {   //go away from attractor
                    vel = Vector3.Lerp(vel, -attractDelta, repulsionWeight * fdt);
                }
            }
       // }
        //owner.rBody.AddForce(vel);
        owner.navAgent.SetDestination(vel);
        owner.transform.LookAt(target.transform);
    }
}