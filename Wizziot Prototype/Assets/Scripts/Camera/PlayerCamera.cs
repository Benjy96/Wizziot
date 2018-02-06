﻿using UnityEngine;

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
    public Vector3 offsetAmounts;

    private Vector3 startPos;
    private Quaternion startRotation;
    private Vector3 offset;    
    private float yawInput = 0f;
    private float pitchInput = 0f;

    private enum CameraMode { Follow, Look }
    private CameraMode State;

    private void Start()
    {
        State = CameraMode.Follow;
        transform.SetParent(player);
        offset = new Vector3(offsetAmounts.x, offsetAmounts.y, offsetAmounts.z);
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
        Vector3 smoothedPos;

        switch (State)
        {
            case CameraMode.Follow:
                //TODO: Save positions between states to revert after switches
                //need a constant value to evaluate against (for the zoomed result values)
                desiredPos = startPos * zoom;
                smoothedPos = Vector3.MoveTowards(transform.localPosition, desiredPos, smoothSpeed * Time.deltaTime);
                transform.localPosition = smoothedPos;
                break;

            case CameraMode.Look:
                //Yaw and pitch control (and offset)
                offset = Quaternion.AngleAxis(yawInput, Vector3.up) * Quaternion.AngleAxis(-pitchInput, Vector3.right) * offset;

                //Smoothing (interpolate between current position, desired position (offsetted and zoomed) with smooth speed)
                desiredPos = player.position + offset * zoom;
                //Max camera pitch
                desiredPos.y = Mathf.Clamp(desiredPos.y, 2f, 15f);
                smoothedPos = Vector3.MoveTowards(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

                //Camera position
                transform.position = smoothedPos;
                transform.LookAt(player.position);
                break;
        }
    }
}