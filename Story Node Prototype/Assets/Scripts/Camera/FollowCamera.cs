using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    private Camera playersView;

	// Use this for initialization
	void Start () {
        playersView = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(new Vector3(playersView.transform.position.x,
            playersView.transform.position.y,
            playersView.transform.position.z)
            );
        transform.Rotate(Vector3.up, 180);
	}
}
