using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereNPC : InteractableNPC
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

    protected override void SetExternalFunctions()
    {
//        //Forgive me father, for I have sinned - using a try/catch like an if statement
//        //Removes need for statics - this allows us to check if we have already bound the external function
//        try
//        {
//            story.InkScript.ValidateExternalBindings();
//        }
//#pragma warning disable CS0168 // Variable is declared but never used
//        catch (Exception e)
//#pragma warning restore CS0168 // Variable is declared but never used
//        {
//            story.InkScript.BindExternalFunction("SpherePushOff", () => SpherePushOff());
//        }

    }

    private void SpherePushOff()
    {
        Vector3 away = (player.transform.position - transform.position).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
