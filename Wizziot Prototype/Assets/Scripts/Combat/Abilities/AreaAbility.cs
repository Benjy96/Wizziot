using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAbility : MonoBehaviour {

    public static float G = 0.5f;

    public float objectMass = 500f;
    public float effectRadius = 5f;
    public float duration = 5f;

    protected Rigidbody rb;
    protected List<Rigidbody> neighbourRbs;

    private void Awake()
    {
        neighbourRbs = new List<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Attract(Rigidbody toAffect)
    {
        if (toAffect == null) return;
        Vector3 direction = rb.position - toAffect.position;

        float distance = Mathf.Clamp(direction.magnitude, 0.001f, float.MaxValue);

        //Calculate Gravitational attraction force based on masses and G
        float forceMagnitude = G * ((rb.mass * toAffect.mass));

        //Apply force t object
        Vector3 force = direction.normalized * forceMagnitude;
        toAffect.AddForce(force);
    }
}
