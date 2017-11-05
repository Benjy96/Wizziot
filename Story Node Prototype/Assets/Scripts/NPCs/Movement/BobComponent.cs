using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobComponent : MonoBehaviour {

    public float bobSpeed;
    public float bobRange;

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = transform.localScale.y + (Mathf.Sin(Time.time * bobSpeed) * bobRange);

        transform.position = pos;
        transform.Rotate(Vector3.up, Mathf.Sin(Time.time));
    }
}
