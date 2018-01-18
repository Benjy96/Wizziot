using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform target;

    public float zoomSpeed = 4f;
    public float yawSpeed = 20f;
    public float maxZoom = 20f;
    public float minZoom = 5f;

    public Vector3 offsetAmounts;

    private Vector3 offset;
    public float zoom = .5f;
    private float yawInput = 0f;

    private Vector3 lastClickPos;

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
    }

    private void LateUpdate()
    {
        offset = Quaternion.AngleAxis(yawInput, Vector3.up) * offset;
        transform.position = (target.position + offset * zoom);
        transform.LookAt(target.position);
    }
}
