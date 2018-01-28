using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vortex : MonoBehaviour
{ 
    public static float G = Singularity.G;  //GameManager.G or StatManager.G in final

    public bool yoyoMode = true; 
    public float vortexMass = 500f;
    public float vortexRadius = 5f;     //This values only used for INITIAL neighbours - can use update method for continuos assignments
    public float duration = 5f;

    private bool vortexForming = false;
    private bool vortexRepulsing = false;
    private Collider[] neighbourhood;
    private List<Rigidbody> neighbourRbs;

    private bool attractStep = true;

    private Rigidbody rb;

    private void Awake()
    {
        neighbourRbs = new List<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    //Once landed on/impacted a surface:
    private void OnCollisionEnter(Collision collision)
    {
        if (!vortexForming)
        {
            //Prevents a secondary collision process if singularity hits 2 objects
            vortexForming = true;

            //Set high mass
            rb.mass = vortexMass;

            neighbourhood = Physics.OverlapSphere(transform.position, vortexRadius);
            foreach (Collider c in neighbourhood)
            {
                if (c != this)
                {
                    Rigidbody temp;
                    //If not part of environment
                    if (c.gameObject.layer == LayerMask.NameToLayer("Environment"))
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
            foreach (Rigidbody body in neighbourRbs)
            {
                Repulse(body);
            }
        }
    }

    //Alternating repulsion/attraction (can add another variable to enable/disable this)
    private void Repulse(Rigidbody toAffect)
    {
        Vector3 direction;
        //Default behaviour is the else (repulse) if yoyoMode not enabled
        if (attractStep && yoyoMode)
        {
            direction = rb.position - toAffect.position;

            float distance = Mathf.Clamp(direction.magnitude, 0.001f, float.MaxValue);

            //Calculate Gravitational attraction force based on masses and G
            float forceMagnitude = G * ((rb.mass * toAffect.mass));

            //Apply force t object
            Vector3 force = direction.normalized * forceMagnitude;
            toAffect.AddForce(force);
        }
        else 
        {
            direction = -(rb.position - toAffect.position);
            //Makes them go up (less emphasis put on x and z direction - y is maintained)
            direction.x *= .5f;
            direction.z *= .5f;

            float distance = Mathf.Clamp(direction.magnitude, 0.001f, float.MaxValue);

            //Calculate Gravitational attraction force based on masses and G
            float forceMagnitude = G * ((rb.mass * toAffect.mass) / Mathf.Pow(distance, 2));

            //Apply force t object
            Vector3 force = direction.normalized * forceMagnitude;
            toAffect.AddForce(force);
        }
        StartCoroutine(Yoyo());
    }

    private IEnumerator Yoyo()
    {
        yield return new WaitForSeconds(.5f);
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
}
