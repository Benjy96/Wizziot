using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;

    public float smoothSpeed;
    public float zoomSpeed;
    public float yawSpeed;
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
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        //Yaw
        if (Input.GetMouseButton(1))
        {
            yawInput = Input.GetAxis("Mouse X") * yawSpeed * Time.deltaTime;
        }

        if (Input.GetMouseButton(1))
        {
            pitchInput = Input.GetAxis("Mouse Y") * yawSpeed * Time.deltaTime;
        }
    }

    //Target will have done movement by the time this is called - no race / concurrency issues
    private void LateUpdate()
    {
        //TODO: add user controlled pitch
        offset = Quaternion.AngleAxis(yawInput, Vector3.up) * offset;
        Vector3 desiredPos = player.position + offset * zoom;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPos;
        transform.LookAt(player.position);
    }
}
