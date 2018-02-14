﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add Parent class for vortex/singularity shared variables and methods
public class Singularity : MonoBehaviour {

    public static float G = 0.5f;

    public float singularityMass = 500f;
    public float singularityRadius = 5f;
    public float duration = 5f;

    public GameObject singularityEffectPrefab;

    private bool singularityForming = false;
    private bool singularityAttracting = false;
    private Collider[] neighbourhood;
    private List<Rigidbody> neighbourRbs;

    private Vector3 startPos;
    private bool raiseObject;

    private Rigidbody rb;

    private void Awake()
    {
        neighbourRbs = new List<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    //Once landed on/impacted a surface:
    private void OnCollisionEnter(Collision collision)
    {
        //e.g. this could be a base.OnCollissionEnter();
        CreateSingularityEffect();

        if (!singularityForming)
        {
            //Prevents a secondary collision process if singularity hits 2 objects
            singularityForming = true;  

            //Set high mass
            rb.mass = singularityMass;

            //Get all items in range
            //TODO: CONSTANT addition? e.g. add items to list in update instead of only once?
                //Maybe as an upgraded singularity - e.g. instead of initial, continual singularity additions
            neighbourhood = Physics.OverlapSphere(transform.position, singularityRadius);
            foreach (Collider c in neighbourhood)
            {
                if (c != this)
                {
                    Rigidbody temp;
                    //If not part of environment
                    if (c.gameObject.layer == LayerMask.NameToLayer(GameMetaInfo._AFFECTABLE_OBJECT_LAYER_NAME))
                    {
                        temp = c.GetComponent<Rigidbody>();
                        if (temp != null)    //Safety check - incase tag forgotten
                        {
                            neighbourRbs.Add(c.GetComponent<Rigidbody>());
                        }
                    }
                }
            }

            //Start process
            StartCoroutine(StartSingularity());
        }
    }

    private void FixedUpdate()
    {
        

        if (singularityAttracting)
        {
            //TODO: Disable movement component of affected NPCs while active
            foreach (Rigidbody body in neighbourRbs)
            {
                Attract(body);
            }
        }
    }

    private void Attract(Rigidbody toAttract)
    {
        //Get Vector from Singularity to object
        Vector3 direction = rb.position - toAttract.position;
        float distance = Mathf.Clamp(direction.magnitude, 0.001f, float.MaxValue);

        //Calculate Gravitational attraction force based on masses and G
        float forceMagnitude = G * ((rb.mass * toAttract.mass) / Mathf.Pow(distance, 2));

        //Apply force t object
        Vector3 force = direction.normalized * forceMagnitude;
        toAttract.AddForce(force);
    }

    private IEnumerator StartSingularity()
    {
        Debug.Log("Starting");
        singularityAttracting = true;
        yield return new WaitForSeconds(duration);
        Debug.Log("Ending");
        singularityAttracting = false;

        //Destroy object
        Destroy(gameObject);
    }

    private void CreateSingularityEffect()
    {
        singularityEffectPrefab = Instantiate(singularityEffectPrefab, transform.position, Quaternion.identity, gameObject.transform);
    }
}
