using UnityEngine;

[CreateAssetMenu(fileName = "Flock State", menuName = "States/Flock")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class FlockState : State {

    //How close to get to target
    [Range(0f, 1f)] public float closenessToTarget = 1f;

    //TODO: Assign to diff script?
    public GameObject target;
    public float collisionAvoidanceWeight = 2f;
    public float velocityMatchingWeight = 0.25f;
    public float flockCenteringWeight = 0.2f;
    public float attractionWeight = 2f;
    public float repulsionWeight = 2f;

    private NeighbourhoodTracker neighbourhood;
    private EnemySpawnPoint spawn;

    private Enemy owner;

    public override State EnterState(Enemy owner)
    {
        this.owner = owner;
        neighbourhood = owner.GetComponent<NeighbourhoodTracker>();
        spawn = owner.Spawn;
        return this;
    }

    public override void Execute()
    {
        float agentSpeed = owner.navAgent.speed;
        //TODO: Implement direction using spawner / neighbourhood attributes, & owner navmesh
        Vector3 vel = owner.Velocity;

        //Collision avoidance - avoid neighbors that are too close
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooClosePos = neighbourhood.AvgTooClosePos;
        if (tooClosePos != Vector3.zero)
        {
            velAvoid = owner.Position - tooClosePos;   //turn away
            velAvoid.Normalize();
            velAvoid *= agentSpeed;
        }

        //Velocity Matching - match velocity of neighbors
        Vector3 velAlign = neighbourhood.AvgVel;
        if (velAlign != Vector3.zero)
        {
            velAlign.Normalize();
            velAlign *= agentSpeed;
        }

        //Flock centering - move towards center of local neighbors
        Vector3 velCenter = neighbourhood.AvgPos;
        if (velCenter != Vector3.zero)
        {
            velCenter -= owner.Position;    //look to center
            velCenter.Normalize();  //direction vector
            velCenter *= agentSpeed;  //set dir magnitude
        }

        //Attraction
        Vector3 delta = target.transform.position - owner.Position;    //Agent to attractor vector
        bool attracted = (delta.sqrMagnitude > (owner.stats.sqrMaxTargetDistance * closenessToTarget));   //Decide whether to be attracted based on NPC sight distance (currently half is optimum)
        Vector3 velAttract = delta.normalized * agentSpeed;   //go in direction of attractor at a velocity

        //Apply ALL velocities - the weighting will help influence how much of an impact "influence" each has. Each vector has an affect since vel is assigned and used each time
        float fdt = Time.fixedDeltaTime;
        if (velAvoid != Vector3.zero)    //if we need to do avoidance
        {
            vel = Vector3.Lerp(vel, velAvoid, collisionAvoidanceWeight * fdt);
        }
        else //else do normal movements
        {
            if (velAlign != Vector3.zero)   //If we need to align
            {
                vel = Vector3.Lerp(vel, velAlign, velocityMatchingWeight * fdt);
            }

            if (velCenter != Vector3.zero)  //If we need to center
            {
                vel = Vector3.Lerp(vel, velCenter, flockCenteringWeight * fdt);
            }

            if (velAttract != Vector3.zero) //If we need to go towards target
            {
                if (attracted)  //if distance from attractor is big enough
                {
                    //lerp from current vector (vel and dir) to *required* attractor vector (vel and dir)
                    vel = Vector3.Lerp(vel, velAttract, attractionWeight * fdt);
                }
                else
                {   //go away from attractor
                    vel = Vector3.Lerp(vel, -velAttract, repulsionWeight * fdt);
                }
            }
        }

        //Set velocity using above calculations
        vel = vel.normalized * agentSpeed;    //update vel velocity after direction has been lerped
        //owner.Velocity = vel;   //Set actual gameobject's velocity vector
        owner.navAgent.SetDestination(vel);
    }
}