using UnityEngine;

//TODO: Link to a config file or options menu (Smoothing controls everything; zoom - zoom speed; rotation speed - pitch & yaw)
public class PlayerCamera : MonoBehaviour {

    public Transform player;

    public float smoothSpeed;
    public float cameraSpeed = 5f;
    public float zoomSpeed = 2f;

    public float maxLockedZoom = 1.7f;
    public float maxZoom;
    public float minZoom;

    public float zoom;

    private Vector3 startPos;
    private Quaternion startRotation;
    private Vector3 offset;    
    private float yawInput = 0f;
    private float pitchInput = 0f;

    public enum CameraMode { Follow, Look }
    public static CameraMode State;

    private void Start()
    {
        State = CameraMode.Follow;
        transform.SetParent(player);
        offset = transform.position;
        startPos = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public void SwitchCameraMode()
    {
        //Switch Camera Mode on "alt" keypress
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            switch (State)
            {
                case CameraMode.Follow:
                    Debug.Log("Camera: Look Mode");
                    transform.SetParent(null, true);
                    State = CameraMode.Look;
                    break;

                case CameraMode.Look:
                    Debug.Log("Camera: Following");
                    transform.SetParent(player, true);
                    transform.localPosition = startPos;
                    transform.localRotation = startRotation;
                    State = CameraMode.Follow;
                    break;
            }
        }
    }

    private void Update()
    {
        //Zoom
        switch (State)
        {
            case CameraMode.Follow:
                zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                zoom = Mathf.Clamp(zoom, minZoom, maxLockedZoom);
                break;

            case CameraMode.Look:
                zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
                break;
        }

        //Yaw
        if (Input.GetMouseButton(1))
        {
            yawInput = Input.GetAxis("Mouse X") * cameraSpeed;
            pitchInput = Input.GetAxis("Mouse Y") * cameraSpeed;
        }
        else
        {
            yawInput = 0f;
            pitchInput = 0f;
        }
    }

    //Target will have done movement by the time this is called - no race / concurrency issues
    private void LateUpdate()
    {
        Vector3 desiredPos;

        switch (State)
        {
            case CameraMode.Follow:
                //TODO: Rotate camera around player's position
                desiredPos = startPos * zoom;
                transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, smoothSpeed * Time.deltaTime);
                break;

            case CameraMode.Look:
                //Yaw and pitch control (and offset)
                offset = Quaternion.AngleAxis(yawInput, Vector3.up) * Quaternion.AngleAxis(-pitchInput, Vector3.right) * offset;

                //Smoothing (interpolate between current position, desired position (offsetted and zoomed) with smooth speed)
                desiredPos = player.position + offset * zoom;
                //Max camera pitch
                desiredPos.y = Mathf.Clamp(desiredPos.y, 2f, 15f);

                //Camera position
                transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, smoothSpeed * Time.deltaTime);
                transform.LookAt(player.position);
                break;
        }
    }
}
