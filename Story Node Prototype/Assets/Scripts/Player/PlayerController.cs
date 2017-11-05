using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    // ----- Configuration Variables ----- //
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
    private InteractableNPC interactingNPC;
    private Projector targetIndicator;
    private Camera cam;

    //Interface
    [Range(0, 225)] public float sqrMaxTargetDistance;

    public float Speed
    {
        get { return playerState.speed; }
    }
    public float TurnSpeed
    {
        get { return playerState.turnSpeed; }
    }

    private void Awake()
    {
        targetIndicator = GetComponentInChildren<Projector>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        HandleDirectionInput();
        HandleTargeting();
        HandleKeyboardInput();
    }

    private void HandleDirectionInput()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        Vector3 velocity = direction * Speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * TurnSpeed);

        transform.Translate(velocity, Space.World);
        transform.LookAt(transform.position + adjustedLook);
    }

    private void HandleTargeting()
    {
        //Must be within 10 metres - using sqr values since getting root is expensive
        if(interactingNPC != null && 
            (interactingNPC.transform.position - transform.position).sqrMagnitude > sqrMaxTargetDistance)
        {
            targetIndicator.enabled = false;
            interactingNPC = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit pointHit;

            if(Physics.Raycast(ray, out pointHit, 100f))    //If the raycast hits something within 100
            {
                if ((pointHit.transform.position - transform.position).sqrMagnitude < sqrMaxTargetDistance)
                {
                    if (pointHit.transform.GetComponent<InteractableNPC>())
                    {
                        if (interactingNPC != null)
                        {
                            targetIndicator.enabled = false;
                        }
                        interactingNPC = pointHit.transform.GetComponent<InteractableNPC>();
                        targetIndicator.transform.SetParent(interactingNPC.transform);
                        targetIndicator.transform.position = interactingNPC.transform.position + new Vector3(0f, interactingNPC.transform.localScale.y * 5);
                        targetIndicator.enabled = true;
                    }
                    else
                    {
                        targetIndicator.enabled = false;
                    }
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (interactingNPC != null) //If we have a target, allow possibility of starting a conversation
        {
            switch (Input.inputString)
            {
                case "f":
                    //Enter a conversation
                    StoryManager.Instance.AttemptToConverse(interactingNPC);
                    break;
            }
        }
    }
}
