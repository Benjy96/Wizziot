using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 3f;
    public float turnSpeed = 3f;

    private float cooldown = 1.5f;
    private bool interacted = false;

    public Interactable sphereNPC;
    //public GameObject sphereNPC;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        float distanceFromSphere = Vector3.Distance(transform.position, sphereNPC.transform.position);
        if (distanceFromSphere <= 5f && interacted == false)
        {
            interacted = true;
            sphereNPC.Interact();
            //sphereNPC.GetComponent<Interactable>().Interact();
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        Vector3 velocity = direction * speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * turnSpeed);

        transform.Translate(velocity, Space.World);
        transform.LookAt(transform.position + adjustedLook);
    }
}
