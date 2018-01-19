using UnityEngine;

//TODO: Link to a config file or options menu (Smoothing controls everything; zoom - zoom speed; rotation speed - pitch & yaw)
public class PlayerCamera : MonoBehaviour {

    public Transform player;

    public float smoothSpeed;
    public float cameraSpeed;
    public float maxZoom;
    public float minZoom;

    public float zoom;
    public Vector3 offsetAmounts;

    private Vector3 offset;    
    private float yawInput = 0f;
    private float pitchInput = 0f;

    private void Start()
    {
        offset = new Vector3(offsetAmounts.x, offsetAmounts.y, offsetAmounts.z);
    }

    private void Update()
    {
        //Zoom
        zoom -= Input.GetAxis("Mouse ScrollWheel") * cameraSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        //Yaw
        if (Input.GetMouseButton(1))
        {
            yawInput = Input.GetAxis("Mouse X") * cameraSpeed;
        }
        else
        {
            yawInput = 0f;
        }

        if (Input.GetMouseButton(1))
        {
            pitchInput = Input.GetAxis("Mouse Y") * cameraSpeed;
        }
        else
        {
            pitchInput = 0f;
        }
    }

    //Target will have done movement by the time this is called - no race / concurrency issues
    private void LateUpdate()
    {
        //Yaw and pitch control (and offset)
        offset = Quaternion.AngleAxis(yawInput, Vector3.up) * Quaternion.AngleAxis(-pitchInput, Vector3.right) * offset;

        //Smoothing (interpolate between current position, desired position (offsetted and zoomed) with smooth speed)
        Vector3 desiredPos = player.position + offset * zoom;
        //Max camera pitch
        desiredPos.y = Mathf.Clamp(desiredPos.y, 2f, 15f);
        Vector3 smoothedPos = Vector3.MoveTowards(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        
        //Camera position
        transform.position = smoothedPos;
        transform.LookAt(player.position);
    }
}
