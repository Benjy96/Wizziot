using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    // ----- State Variables ----- //
    public PlayerStateData playerState; //Player state manages the player's current state, and will be saved (if the game supports saving).

    [Serializable]
    public class PlayerStateData
    {
        [Range(0, 10)] public float speed;
        [Range(50, 150)] public float turnSpeed;
        [Range(0, 225)] public float sqrMaxTargetDistance;
    }

    // ---- Book-keeping Fields ----- //    //Convenience properties and variables, plus variables that do not need saved.
    //Player MODE
    //TODO: Use state machine when needed
    //private enum PlayerState { Normal, Combat, Conversing, Dead }
    //PlayerState State;  
        
    //Interaction
    private TargetType currentTargetType;   //State machine to hold player's targeting status for if/switches
    private Targetable interactionTarget;
    private InteractableNPC interactableNPC;
    //TODO: Add items
    //TODO: Add enemies
    private Projector targetIndicator;
    private Camera cam;

    //Lasers
    public GameObject destroyFX;
    public float fireRate = .25f;
    private LineRenderer laserLine;
    private WaitForSeconds spellDuration = new WaitForSeconds(0.07f);
    private float nextFire; //track time passed

    public float Speed
    {
        get { return playerState.speed; }
    }
    public float TurnSpeed
    {
        get { return playerState.turnSpeed; }
    }

    private void Awake()    //References & initialisation - Start is once scripts are definitely enabled - Awake the GOs are all enabled
    {
        targetIndicator = GetComponentInChildren<Projector>();
        laserLine = GetComponent<LineRenderer>();
        cam = Camera.main;

        nextFire = 0;
    }

    void Update () {
        HandleDirectionInput();
        HandleTargeting();
        HandleKeyboardInput();
        HandleConversationInput();
        //HandleShoot();
    }

    private void HandleShoot()  //TODO: could make this (and all abils) components - add them to player when u unlock the abil (and bind it to a key?)
    {
        if (interactionTarget != null) //TODO: make so can't shoot story NPCs 
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                StartCoroutine(ShotEffect());

                Vector3 rayOrigin = transform.position;

                laserLine.SetPosition(0, rayOrigin);
                laserLine.SetPosition(1, interactionTarget.transform.position);
                Instantiate(destroyFX, interactionTarget.transform.position, Quaternion.identity);
                Destroy(interactionTarget.gameObject); //TODO: remove <- not final implementation - destroys any nested components
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        laserLine.enabled = true;
        yield return spellDuration;
        laserLine.enabled = false;
    }

    private void HandleDirectionInput()
    {
        //Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));   //get our directions
        //Vector3 direction = input.normalized;   //normalize the direction (for when both are held at same time)
        //Vector3 velocity = direction * Speed * Time.deltaTime;  //get magnitude by multiplying by speed, then scale to time
        //Vector3 adjustedLook = Vector3.Lerp(transform.forward, direction, Time.deltaTime * TurnSpeed);

        //transform.Translate(velocity, Space.World);
        //transform.LookAt(transform.position + adjustedLook);

        float x = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    private void HandleTargeting()
    {
        //Must be within 10 metres - using sqr values since getting root is expensive
        if(interactionTarget != null && 
            (interactionTarget.transform.position - transform.position).sqrMagnitude > playerState.sqrMaxTargetDistance)
        {
            targetIndicator.enabled = false;
            interactionTarget = null;
            StoryManager.Instance.CloseConversation();
        }

        //TARGET OPERATION (LMB)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit pointHit;

            //Raycast
            if(Physics.Raycast(ray, out pointHit, 100f))    //If the raycast hits something within 100
            {
                //If within range
                if ((pointHit.transform.position - transform.position).sqrMagnitude < playerState.sqrMaxTargetDistance)
                {

                    if (pointHit.transform.GetComponent<Targetable>() != null)  
                    {
                        currentTargetType = pointHit.transform.GetComponent<Targetable>().targetType;
                        switch (currentTargetType)
                        {
                            case TargetType.Item:
                                //TODO: Add item targeting code
                                break;

                            case TargetType.Enemy:
                                //TODO: Add enemy targeting code
                                break;

                            case TargetType.Story:
                                if (interactionTarget != null)
                                {
                                    targetIndicator.enabled = false;
                                }
                                interactionTarget = pointHit.transform.GetComponent<InteractableNPC>();
                                targetIndicator.transform.SetParent(interactionTarget.transform);
                                targetIndicator.transform.position = interactionTarget.transform.position + new Vector3(0f, interactionTarget.transform.localScale.y * 5);
                                targetIndicator.enabled = true;
                                break;

                            default:
                                targetIndicator.enabled = false;
                                break;
                        }
                    }
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (interactionTarget != null && interactionTarget.GetComponent<InteractableNPC>() != null) //If we have a target, allow possibility of starting a conversation
        {
            switch (Input.inputString)
            {
                case "f":
                    //Enter a conversation
                    StoryManager.Instance.AttemptToConverse(interactionTarget.GetComponent<InteractableNPC>());
                    break;
            }
        }
    }

    private void HandleConversationInput()
    {
        if (interactionTarget != null)
        {
            int playerChoice;
            bool correctInput = int.TryParse(Input.inputString, out playerChoice);
            if (correctInput)
            {
                StoryManager.Instance.Choice(playerChoice);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StoryManager.Instance.CloseConversation();
            }
        }
    }
}
