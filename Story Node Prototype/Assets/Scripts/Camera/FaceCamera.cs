using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    private Camera playersView;

	// Use this for initialization
	void Start () {
        playersView = Camera.main;
	}
	
	// Look at the camera (only rotate about one axis)
	void Update () {
        //Rotate ABOUT the x axis (gets the y and z of target - stays at same x - this rotates around the x)
        Vector3 targetPos = new Vector3(playersView.transform.position.x, playersView.transform.position.y, playersView.transform.position.z);
        transform.LookAt(targetPos);
        transform.Rotate(Vector3.up, 180);  //Flip text from back to front
	}
}
