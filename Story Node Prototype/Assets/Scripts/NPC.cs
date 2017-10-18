using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : InteractableNPC {

    [Range(0f, 10f)]
    public float bobSpeed = 3f;
    [Range(0f, 0.5f)]
    public float bobRange = .5f;
    public float pushOffForce = 20f;

    private Vector3 InteractingNPC
    {
        get
        {
            return player.interactingNPC.transform.position;
        }
    }

    #region MonoBehaviours
    private void Awake()
    {
        if(player == null) player = FindObjectOfType<PlayerController>();
    }
    
    //External functions MUST be set in start -- eliminates "Race" condition - inkStory is set in awake
    private void Start()
    {
        SetExternalFunctions();
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = transform.localScale.y + (Mathf.Sin(Time.time * bobSpeed) * bobRange);

        transform.position = pos;
        transform.Rotate(Vector3.up, Mathf.Sin(Time.time));
    }
    #endregion

    public override void Interact()
    {
        story._inkStory.ChoosePathString(inkPath);
        story.DoStory();
    }

    public override void SetExternalFunctions()
    {
        //Forgive me father, for I have sinned
        //Removes need for statics - this allows us to check if we have already bound the external function
        try
        {
            story._inkStory.ValidateExternalBindings();
        }catch(Exception e)
        {
            story._inkStory.BindExternalFunction("PushOff", () => PushOff());
        }
        
    }

    private void PushOff()
    {
        Vector3 away = (player.transform.position - new Vector3(InteractingNPC.x, InteractingNPC.y, InteractingNPC.z)).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
