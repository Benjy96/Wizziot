using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeNPC : InteractableNPC
{
    #region MonoBehaviours
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = transform.localScale.y + (Mathf.Sin(Time.time * bobSpeed) * bobRange);

        transform.position = pos;
        transform.Rotate(Vector3.up, Mathf.Sin(Time.time));
    }
    #endregion

    protected override void RegisterExternalFunctions()
    {
        storyManager.BindExternalFunction("CubePushOff", CubePushOff);
    }

    private void CubePushOff()
    {
        Vector3 away = (player.transform.position - transform.position).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
