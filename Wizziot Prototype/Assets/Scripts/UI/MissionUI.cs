using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour {

    public Text title;
    public Text body;
    public RectTransform compass;

    private Vector3 currentWaypoint;

    public void SetMissionText(Mission mission)
    {
        title.text = mission.title;
        currentWaypoint = mission.location;

        if (mission.GetType() == typeof(KillMission))
        {
            KillMission kMission = (KillMission)mission;
            int killRequiredIndex = 0;  //Track list of enemies / kills required per enemy type (each list index corresponds, e.g. enemy type 01 -> kill 5. Each stored in 2 lists.
            body.text = ""; //Replace placeholder text
            foreach (GameObject x in kMission.killTypes)
            {
                if (killRequiredIndex > 0) body.text += "\n";
                body.text += x.name + " To Kill: " + kMission.killsRequired[killRequiredIndex];
                killRequiredIndex++;
            }
        }
        else if(mission.GetType() == typeof(CollectMission))
        {
            CollectMission cMission = (CollectMission)mission;
            body.text = ""; //Replace placeholder text
            body.text = "Find: " + cMission.collectItem;
        }
        else //normal mission
        {
            body.text = "";
            body.text = "Go to Waypoint: " + currentWaypoint;
        }
    }
    //Unit circle & trigonometry: 
    //https://www.khanacademy.org/math/algebra2/trig-functions/unit-circle-definition-of-trig-functions-alg2/a/trig-unit-circle-review
    //Rotate compass - rotates differently depending on Camera State (Look or Follow)
    //Calculating a 2D rotation from a 3D direction
    /** 
    * 1. Normalize a vector to get a point on the "Unit Circle"
    *    (Disregard one axis - y - to make it a 2D direction)
    *    (y is height in 3D, doesn't matter for direction to a waypoint
    *    since waypoints are infinite in height, only horizontal matters)
    * 2. Use the x and z values of direction to calculate hypoteneuse
    *    between the player and waypoint
    * 3. Use trigonometry to get angle to waypoint
    * 4. Set camera angle to this angle, adjusted
    * */
    private void Update()
    {
        if(currentWaypoint != null)
        {
            
            Quaternion q = new Quaternion();

            //If camera locked, rotate compass depending on player rotation
            if (PlayerCamera.State == PlayerCamera.CameraMode.Follow)
            {
                //Get player's world co-ordinates
                Transform player = PlayerManager.Instance.player.GetComponent<Transform>();

                Vector3 playerPos = player.position;
                //1: Scale vector to unit circle size
                Vector3 direction = (currentWaypoint - playerPos).normalized;
                //2: Get hypoteneuse for trigonometry. 
                float hypoteneuse = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));
                //3: Angle to waypoint. SOH CAH TOA: Cos(Adjacent/Hypoteneuse)
                float angle = Mathf.Acos(direction.z / hypoteneuse) * Mathf.Rad2Deg;

                //For rotating the compass depending on the player's rotation, rather than the world's
                float playerFaceAngle = player.rotation.eulerAngles.y;

                //Trig functions are repeating, so past 180 degrees we need to flip the angle
                if (direction.x < 0)
                {
                    //4: playerFaceAngle makes rotation depend upon the player's rotation, 
                    //not the "forward" direction of the Unity scene (+ playerFaceAngle)
                    q = Quaternion.AngleAxis(angle + playerFaceAngle, Vector3.forward);
                }
                else
                {
                    q = Quaternion.AngleAxis(-angle + playerFaceAngle, Vector3.forward);
                }
            }
            //Same algorithm but based on camera position rather than player
            else if (PlayerCamera.State == PlayerCamera.CameraMode.Look)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 direction = (currentWaypoint - camPos).normalized;

                float hypoteneuse = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));  //Used for trig (calculating angle - Cos = A/H)
                float angle = Mathf.Acos(direction.z / hypoteneuse) * Mathf.Rad2Deg;    //Angle to waypoint

                //Use this for rotating the compass DEPENDING on the camera's rotation, rather than the world's
                float camFaceAngle = Camera.main.transform.rotation.eulerAngles.y;

                if (direction.x < 0)
                {
                    q = Quaternion.AngleAxis(angle + camFaceAngle, Vector3.forward);
                }
                else
                {
                    q = Quaternion.AngleAxis(-angle + camFaceAngle, Vector3.forward);
                }
            }   
            
            //5: Rotate the compass
            compass.rotation = q;
        }
    }//Update(){}
}
