using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    // ----- Configuration Variables ----- //
    public PlayerConfigurationData playerConfig;

    [Serializable]
    public class PlayerConfigurationData
    {
        public float speed;
        public float turnSpeed;
    }

    // ----- State Variables ----- //
    public PlayerStateData playerState;

    [Serializable]
    public class PlayerStateData
    {
        //public float health;
    }

    // ---- Book Keeping Fields ----- //
    private bool interacted = false;
    public InteractableNPC interactingNPC;

    public float Speed
    {
        get { return playerConfig.speed; }
    }
    public float TurnSpeed
    {
        get { return playerConfig.turnSpeed; }
    }

    #region MonoBehaviour
    // Update is called once per frame
    void Update () {
        if (interactingNPC != null)
        {
            float distanceFromNPC = Vector3.Distance(transform.position, interactingNPC.transform.position);
            if (distanceFromNPC <= 5f && interacted == false)
            {
                interacted = true;
                interactingNPC.Interact();
            }
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        Vector3 velocity = direction * Speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * TurnSpeed);

        transform.Translate(velocity, Space.World);
        transform.LookAt(transform.position + adjustedLook);
    }
#endregion MonoBehaviour

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
