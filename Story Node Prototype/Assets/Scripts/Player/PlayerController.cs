using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    // ----- Configuration Variables ----- //
    public PlayerConfigurationData playerConfig;    //Config data "defines" our object's "attributes" and is treated as a constant. Set in inspector.

    [Serializable]
    public class PlayerConfigurationData
    {
        public float maxSpeed;
        public float maxTurnSpeed;
    }

    // ----- State Variables ----- //
    public PlayerStateData playerState; //Player state manages the player's current state, and will be saved (if the game supports saving).

    [Serializable]
    public class PlayerStateData
    {
        public float speed;
        public float turnSpeed;
    }

    // ---- Book-keeping Fields ----- //    //Convenience properties and variables, plus variables that do not need saved.
    //Implementation Data
    private bool interacted = false;
    private InteractableNPC interactingNPC;

    //Interface
    public float Speed
    {
        get { return playerState.speed; }
        set { if (value < playerConfig.maxSpeed) playerState.speed = value; }
    }
    public float TurnSpeed
    {
        get { return playerState.turnSpeed; }
        set { if (value < playerConfig.maxTurnSpeed) playerState.turnSpeed = value; }
    }

    public Vector3 TargetPos
    {
        get { return interactingNPC.transform.position; }
    }

    #region MonoBehaviour
    // Update is called once per frame
    void Update () {
        HandleInput();
        HandleInteraction();
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

    private void HandleInput()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        Vector3 velocity = direction * Speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * TurnSpeed);

        transform.Translate(velocity, Space.World);
        transform.LookAt(transform.position + adjustedLook);
    }

    private void HandleInteraction()
    {
        if (interactingNPC != null)
        {
            float distanceFromNPC = Vector3.Distance(transform.position, interactingNPC.transform.position);
            if (distanceFromNPC <= 5f && interacted == false)
            {
                interacted = true;
                interactingNPC.Interact();
            }
        }
    }
}
