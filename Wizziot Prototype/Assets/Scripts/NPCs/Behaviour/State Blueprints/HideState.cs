using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "HideState", menuName = "States/Hide")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class HideState : State {

    private Transform chaser;
    private Vector3 hideSpot;
    private float chaserSightDist;

    private Transform hideObstacle;
    private float checkHiddenInterval;
    private float checkHiddenIncrements = 10f;

    protected override void EnterState(Enemy owner, GameObject lastInfluence)
    {
        base.EnterState(owner, lastInfluence);

        neighbourhoodTracker.TrackObstacles();
        checkHiddenInterval = Time.time + checkHiddenIncrements;
    }

    /**
        1.) Seeker.pos - object.pos = direction through object
        2.) Normalize for direction vector
        3.) Set magnitude to a factor of object radius (so hide position is not inside object)
        4.) Move agent to calculated hide position.

        Example Modifications:
    1 - Hide only if in view. 2 - Favour rear of target. 3 - Hide only if under threat.
    */
    public override void Execute()
    {
        hideSpot = Vector3.forward;
        chaser = SelectTarget();

        if (chaser != null && chaser.GetComponent<AgentStats>() != null) chaserSightDist = chaser.GetComponent<AgentStats>().sqrMaxTargetDistance;

        //If current hiding spot not letting us hide from player, re-evaluate
        if (chaser != null && owner.CanSeeTarget(chaser))
        {
            owner.Influence(Emotion.Fear, 1f * Time.deltaTime);

            if (hideObstacle == null) hideObstacle = GetNewHideObstacle();

            //If it's been too long and we aren't able to hide, change hiding spot or enrage (Check can hide)
            if ((Time.time > checkHiddenInterval))
            {
                //If no obstacles, enrage. Get new obstacle. If obstacle already selected, enrage. Else continue.
                if (VerifiedNoObstacles()) owner.Enrage();
                Transform tempObstacle = GetNewHideObstacle();
                if (tempObstacle == hideObstacle) owner.Enrage();
                else hideObstacle = tempObstacle;
                checkHiddenInterval = Time.time + checkHiddenIncrements;
            }

            //Calculate & move to a hide spot
            if(hideObstacle != null) hideSpot = CalculateHideSpot(hideObstacle, chaser);
            if (PointHiddenFromChaser(hideSpot))
            {
                owner.MoveTo(hideSpot);
            }
        }
        else
        {
            owner.Influence(Emotion.Calm, .2f * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Returns true if a ray hits an obstacle before the point
    /// </summary>
    private bool PointHiddenFromChaser(Vector3 point)
    {
        Ray ray = new Ray(point, chaser.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, chaserSightDist))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a point behind an obstacle but opposite the chaser.
    /// </summary>
    private Vector3 CalculateHideSpot(Transform point, Transform chaser)
    {
        Vector3 direction = (point.position - chaser.position).normalized;

        float obstacleSizeX = point.transform.localScale.x;
        float obstacleSizeZ = point.transform.localScale.z;
        float biggest = (obstacleSizeX > obstacleSizeZ) ? obstacleSizeX : obstacleSizeZ;
        Vector3 adjustedForObstacleSizePos = point.transform.position + (direction * biggest / 2);

        return adjustedForObstacleSizePos;
    }

    /// <summary>
    /// Returns an obstacle that blocks the chaser's vision
    /// </summary>
    /// <returns></returns>
    private Transform GetNewHideObstacle()
    {
        foreach (Transform pos in neighbourhoodTracker.obstacles)
        {
            if (pos == hideObstacle) continue;
            if (PointHiddenFromChaser(pos.transform.position))
            {
                Debug.Log("Point behind pos is hidden");
                //Calculate spot
                return pos;
            }
        }
        return null;
    }

    private bool VerifiedNoObstacles()
    {
        if(neighbourhoodTracker.obstacles.Count == 0)
        {
            neighbourhoodTracker.ScanForNearby();
            if (neighbourhoodTracker.obstacles.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
