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

    protected override void EnterState(Enemy owner, GameObject lastInfluence)
    {
        base.EnterState(owner, lastInfluence);

        neighbourhoodTracker.TrackObstacles();
        checkHiddenInterval = Time.time + 10f;
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

        if (chaser.GetComponent<EntityStats>() != null) chaserSightDist = chaser.GetComponent<EntityStats>().sqrMaxTargetDistance;

        //If current hiding spot not letting us hide from player, re-evaluate
        if (chaser != null && owner.CanSeeTarget(chaser))
        {
            owner.Influence(Emotion.Fear, 1f * Time.deltaTime);

            if (hideObstacle == null) hideObstacle = GetNewHideObstacle();

            //If it's been too long and we aren't hidden, change hiding spot
            if ((Time.deltaTime > checkHiddenInterval))
            {
                hideObstacle = GetNewHideObstacle();
                
            }
            //Calculate & move to a hide spot
            hideSpot = CalculateHideSpot(GetNewHideObstacle(), chaser);
            if (PointHiddenFromChaser(hideSpot))
            {
                owner.MoveTo(hideSpot);
            }
        }
        else
        {
            owner.Influence(Emotion.Calm, .2f * Time.fixedDeltaTime);
        }

        //If been trying to hide for a long time, and nowhere to hide, influence maximum fear, else add to interval
        if (Time.time > checkHiddenInterval && VerifiedNoObstacles())
        {
            owner.Enrage();
        }
        else if (Time.time > checkHiddenInterval)
        {
            checkHiddenInterval = Time.time + 10f;
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
            Debug.Log("Chaser can't see point");
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
