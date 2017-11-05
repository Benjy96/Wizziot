using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereNPC : InteractableNPC
{
    public float pushOffForce = 5f;

    protected override void RegisterExternalFunctions()
    {
        storyManager.BindExternalFunction("SpherePushOff", SpherePushOff);
    }

    private void SpherePushOff()
    {
        Vector3 away = (player.transform.position - transform.position).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
