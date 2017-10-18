using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {

    [Range(0f, 10f)]
    public float bobSpeed = 3f;
    [Range(0f, 0.5f)]
    public float bobRange = .5f;

    #region MonoBehaviours
    private void Awake()
    {
        story.PushOff = PushOff;
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = transform.localScale.y + (Mathf.Sin(Time.time * bobSpeed) * bobRange);

        transform.position = pos;
        transform.Rotate(Vector3.up, Mathf.Sin(Time.time));
    }
    #endregion

    private void PushOff()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        Vector3 away = (player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z)).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }

    public override void Interact()
    {
        story._inkStory.ChoosePathString(inkPath);
        story.DoStory();
    }
}
