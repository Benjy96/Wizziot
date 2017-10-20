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
        public float maxSpeed;
        public float maxTurnSpeed;
    }

    // ----- State Variables ----- //
    public PlayerStateData playerState;

    [Serializable]
    public class PlayerStateData
    {
        public float speed;
        public float turnSpeed;
    }

    // ---- Book-keeping Fields ----- //
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
