using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add Parent class for vortex/singularity shared variables and methods
public class Singularity : AreaAbility {

    public GameObject singularityEffectPrefab;
    private bool prefabPlaced = false;

    private bool singularityForming = false;
    private bool singularityAttracting = false;
    private Collider[] neighbourhood;

    private Vector3 startPos;
    private bool raiseObject;

    private void Awake()
    {
        neighbourRbs = new List<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    //Once landed on/impacted a surface:
    private void OnCollisionEnter(Collision collision)
    {
        //e.g. this could be a base.OnCollissionEnter();
        if(!prefabPlaced) CreateSingularityEffect();

        if (!singularityForming)
        {
            //Prevents a secondary collision process if singularity hits 2 objects
            singularityForming = true;  

            //Set high mass
            rb.mass = objectMass;

            //Get all items in range
            //TODO: CONSTANT addition? e.g. add items to list in update instead of only once?
                //Maybe as an upgraded singularity - e.g. instead of initial, continual singularity additions
            neighbourhood = Physics.OverlapSphere(transform.position, effectRadius);
            foreach (Collider c in neighbourhood)
            {
                if (c != this)
                {
                    Rigidbody temp;
                    //If not part of environment
                    if (c.gameObject.layer == LayerMask.NameToLayer(GameMetaInfo._LAYER_AFFECTABLE_OBJECT))
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
        prefabPlaced = true;
        singularityEffectPrefab = Instantiate(singularityEffectPrefab, transform.position, Quaternion.identity, gameObject.transform);
    }
}
