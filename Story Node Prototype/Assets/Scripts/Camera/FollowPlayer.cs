using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform target;
    public Vector3 offset;
    public float zoomSpeed = 4f;
    public float maxZoom = 20f;
    public float minZoom = 5f;

    private float zoom = 10f;

    private void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    private void LateUpdate()
    {
        Vector3 dir = target.position - offset * zoom;
        transform.position = new Vector3(dir.x, target.position.y + dir.y, dir.z);
    }
}
