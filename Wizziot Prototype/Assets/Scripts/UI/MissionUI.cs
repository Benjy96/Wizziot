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

    //Rotate compass - rotates differently depending on Camera State (Look or Follow) - a bunch of trigonometry, so beware
    private void Update()
    {
        if(currentWaypoint != null)
        {
            Transform player = PlayerManager.Instance.player.GetComponent<Transform>();

            Vector3 playerPos = player.position;
            Vector3 direction = (currentWaypoint - playerPos).normalized;

            //Unit circle & trigonometry: https://www.khanacademy.org/math/algebra2/trig-functions/unit-circle-definition-of-trig-functions-alg2/a/trig-unit-circle-review
            /** Get point on 2D unit circle:
             * Think of the 3D world from above as a graph
             * Direction vector made up of x and z (x and y of a 2d graph)
             * We then use the x and y to calculate the hypoteneuse
             * Using the hypoteneuse we then get the angle between the player and waypoint using (inverse) trigonometry (SOH CAH TOA)
             * Then use this angle to rotate the compass
             * */
            float hypoteneuse = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));  //Used for trig (calculating angle - Cos = A/H)
            float angle = Mathf.Acos(direction.z / hypoteneuse) * Mathf.Rad2Deg;    //Angle to waypoint

            Quaternion q = new Quaternion();

            //Correct rotation (trig are repeating functions - need to flip the compass when on "other side" of waypoint)
            if (PlayerCamera.State == PlayerCamera.CameraMode.Follow)
            {
                //Use this for rotating the compass DEPENDING on the player's rotation, rather than the world's
                float playerFaceAngle = player.rotation.eulerAngles.y;

                //Trig functions are repeating, so past 180 degrees we need to flip the angle
                if (direction.x < 0)
                {
                    //Depend upon the player's rotation, not the "forward" direction of the Unity scene (+ playerFaceAngle)
                    q = Quaternion.AngleAxis(angle + playerFaceAngle, Vector3.forward);
                }
                else
                {
                    q = Quaternion.AngleAxis(-angle + playerFaceAngle, Vector3.forward);
                }
            }
            else if (PlayerCamera.State == PlayerCamera.CameraMode.Look)
            {
                if (direction.x < 0)
                {
                    q = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                else
                {
                    q = Quaternion.AngleAxis(-angle, Vector3.forward);
                }
            }
            
            compass.rotation = q;
        }
    }
}
