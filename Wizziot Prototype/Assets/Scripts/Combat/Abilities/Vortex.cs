using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vortex : AreaAbility
{
    public bool yoyoMode = true;

    public WaitForSeconds yoyoDuration = new WaitForSeconds(1f);

    public GameObject vortexEffectPrefab;

    private bool vortexForming = false;
    private bool vortexRepulsing = false;
    private Collider[] neighbourhood;
    
    private bool attractStep = true;

    //Once landed on/impacted a surface:
    private void OnCollisionEnter(Collision collision)
    {
        CreateVortexEffect();

        if (!vortexForming)
        {
            //Prevents a secondary collision process if singularity hits 2 objects
            vortexForming = true;

            //Set high mass
            rb.mass = objectMass;

            neighbourhood = Physics.OverlapSphere(transform.position, effectRadius);
            foreach (Collider c in neighbourhood)
            {
                if (c != gameObject.GetComponent<Collider>())
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
            StartCoroutine(StartVortex());
        }
    }

    private void FixedUpdate()
    {
        if (vortexRepulsing)
        {
            //TODO: Disable movement component of affected NPCs while active & re-enable
            StartCoroutine(Yoyo());
            foreach (Rigidbody body in neighbourRbs)
            {
                ApplyVortex(body);
            }
        }
    }

    //Alternating repulsion/attraction (can add another variable to enable/disable this)
    private void ApplyVortex(Rigidbody toAffect)
    {
        if (toAffect == null) return;
        //Default behaviour is the else (repulse) if yoyoMode not enabled
        if (attractStep && yoyoMode)
        {
            Attract(toAffect);
        }
        else 
        {
            Repulse(toAffect);
        }
    }

    private IEnumerator Yoyo()
    {
        yield return yoyoDuration;
        if (attractStep)
        {
            attractStep = false;
        }
        else
        {
            attractStep = true;
        }
    }

    private IEnumerator StartVortex()
    {
        Debug.Log("Starting");
        vortexRepulsing = true;
        yield return new WaitForSeconds(duration);
        Debug.Log("Ending");
        vortexRepulsing = false;

        //Destroy object
        Destroy(gameObject);
    }

    private void CreateVortexEffect()
    {
        vortexEffectPrefab = Instantiate(vortexEffectPrefab, transform.position, Quaternion.identity, gameObject.transform);
    }
}
