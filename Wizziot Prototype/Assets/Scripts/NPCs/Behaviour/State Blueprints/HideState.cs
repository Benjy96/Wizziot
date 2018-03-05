using UnityEngine;

[CreateAssetMenu(fileName = "HideState", menuName = "States/Hide")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class HideState : State {

    private Transform chaser;
    private Vector3 newHideSpot;
    private float chaserSightDist;

    private Transform hideObstacle;

    protected override void EnterState(Enemy owner, GameObject lastInfluence)
    {
        base.EnterState(owner, lastInfluence);

        neighbourhoodTracker.TrackObstacles();
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
        newHideSpot = Vector3.zero;
        chaser = SelectTarget();
        if (chaser != null)
        {
            EntityStats eS = chaser.GetComponent<EntityStats>();
            if (eS != null) chaserSightDist = eS.sqrMaxTargetDistance;
            CalculateHideSpot();
        }
        else
        {
            //owner.Influence(Emotion.Calm, 0.2f * Time.deltaTime);
        }
    }

    private void CalculateHideSpot()
    {
        Vector3 distance = Vector3.zero;

        if (CanSeeChaser())
        {
            Debug.Log("Can see chaser, hiding");
            //Check appropriate hide spots
            if (neighbourhoodTracker.obstacles == null) neighbourhoodTracker.ScanForNearby();
            foreach (Transform pos in neighbourhoodTracker.obstacles) //List is already sorted, closest will be first!
            {
                Debug.Log("Checking neighbourhood, pos found: " + pos.name);
                if (PointHiddenFromChaser(pos.transform.position))
                {
                    Debug.Log("Point behind pos is hidden");
                    //Calculate spot
                    newHideSpot = CalculateHideSpot(pos, chaser);
                    owner.MoveTo(newHideSpot);
                    owner.FaceTarget(chaser.position);

                    hideObstacle = pos;
                    break;
                }
            }
        }
        else if(hideObstacle != null)
        {
            //TODO: timer - if enemy sees for too long, hide somewhere else (coroutine - for getting stuck, say on rocks)
            owner.Influence(Emotion.Calm, .2f * Time.deltaTime);
            //Mirror enemy movement about obstacle
            newHideSpot = CalculateHideSpot(hideObstacle, chaser);
            owner.MoveTo(newHideSpot);
        }
    }

    private bool CanSeeChaser()
    {
        Ray ray = new Ray(owner.Position, (chaser.position - owner.Position).normalized);
        RaycastHit hit;
        //Racyast everything except affectable objects
        if(Physics.Raycast(ray, out hit, owner.SightRange, LayerMask.GetMask("Default", GameMetaInfo._LAYER_IMMOVABLE_OBJECT), QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag.Equals(chaser.tag))
            {
                Debug.Log("Can see: " + hit.transform.tag);
                owner.Influence(Emotion.Fear, 1f);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private bool PointHiddenFromChaser(Vector3 point)
    {
        Ray ray = new Ray(owner.Position, chaser.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, chaserSightDist))
        {
            Debug.Log("Chaser can't see point");
            return true;
        }
        return false;
    }

    private Vector3 CalculateHideSpot(Transform point, Transform chaser)
    {
        Vector3 direction = (point.position - chaser.position).normalized;

        float obstacleSizeX = point.transform.localScale.x;
        float obstacleSizeZ = point.transform.localScale.z;
        float biggest = (obstacleSizeX > obstacleSizeZ) ? obstacleSizeX : obstacleSizeZ;
        Vector3 adjustedForObstacleSizePos = point.transform.position + (direction * biggest / 2);

        return adjustedForObstacleSizePos;
    }
}
