using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 3f;
    public float turnSpeed = 3f;

    private bool interacted = false;

    public InteractableNPC interactingNPC;
    //TODO: Check differences in utility/(performance?) between GameObject/Interactable
    //public GameObject sphereNPC;
	
	// Update is called once per frame
	void Update () {
        if (interactingNPC != null)
        {
            float distanceFromNPC = Vector3.Distance(transform.position, interactingNPC.transform.position);
            if (distanceFromNPC <= 5f && interacted == false)
            {
                interacted = true;
                interactingNPC.Interact();
                //TODO: Check differences in utility/(performance?) between GameObject/Interactable 
                //sphereNPC.GetComponent<Interactable>().Interact();
            }
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        Vector3 velocity = direction * speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * turnSpeed);

        transform.Translate(velocity, Space.World);
        transform.LookAt(transform.position + adjustedLook);
    }

    private void OnTriggerEnter(Collider other)
    {
        interactingNPC = other.gameObject.GetComponent<InteractableNPC>();
    }

    private void OnTriggerExit(Collider other)
    {
        if(interacted) interacted = false;
        interactingNPC = null;
    }
}
