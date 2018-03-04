using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour {

    public Text title;
    public Text body;
    public RectTransform compass;

    private Vector3 currentWaypoint;

    public void ActivateMission(Mission mission)
    {
        title.text = mission.name;
        body.text = "test";
        currentWaypoint = mission.location;
    }

    private void Update()
    {
        if(currentWaypoint != null)
        {
            //TODO: Change compass depending on camera position

            Transform player = PlayerManager.Instance.player.GetComponent<Transform>();

            Vector3 playerPos = player.position;
            Vector3 direction = (currentWaypoint - playerPos).normalized;

            //Unit circle trigonometry: https://www.khanacademy.org/math/algebra2/trig-functions/unit-circle-definition-of-trig-functions-alg2/a/trig-unit-circle-review
            //Get point on 2D unit circle
            float hypoteneuse = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));  //Used for trig (calculating angle - Cos = A/H)
            float angle = Mathf.Acos(direction.z / hypoteneuse) * Mathf.Rad2Deg;    //Angle to waypoint

            Quaternion q = new Quaternion();

            //Correct rotation (trig are repeating functions - need to flip the compass when on "other side" of waypoint)
            if(direction.x < 0)
            {
                q = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                q = Quaternion.AngleAxis(-angle, Vector3.forward);
            }
            
            compass.rotation = q;
        }
    }
}
