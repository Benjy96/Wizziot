using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : MonoBehaviour {

    public float angleToLookEachSide = 15f;
    [Range(0f, 1f)] public float speed = 1f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (transform.rotation.y > angleToLookEachSide)
        {
            transform.Rotate(Vector3.up, Mathf.Sin(Time.time) * speed);
        }
        else
        {
            transform.Rotate(Vector3.up, Mathf.Cos(Time.time) * speed);
        }
    }
}
