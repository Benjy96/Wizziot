using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {

    [Range(0f, 10f)]
    public float bobSpeed = 3f;
    [Range(0f, 0.5f)]
    public float bobRange = .5f;

    private Vector3 InteractingNPC
    {
        get
        {
            return player.interactingNPC.transform.position;
        }
    }

    private void Awake()
    {
        if(player == null) player = FindObjectOfType<PlayerController>();
    }

    #region MonoBehaviours
    private void Start()
    {
        if (externalsSet == false)
        {
            SetExternalFunctions();
            externalsSet = true;
        }
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
        story._inkStory.BindExternalFunction("PushOff", () => PushOff());
    }

    private void PushOff()
    {
        Vector3 away = (player.transform.position - new Vector3(InteractingNPC.x, InteractingNPC.y, InteractingNPC.z)).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
