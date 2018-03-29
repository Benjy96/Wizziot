using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour {

    float startY;
    [Range(0f, 1f)] public float hoverRange = 1f;

    private void Start()
    {
        startY = transform.position.y;
    }

    private void Update()
    {
        //Speed is time, range is hoverRange
        transform.position = new Vector3(transform.position.x, startY + Mathf.Sin(Time.time) * hoverRange, transform.position.z);
    }
}
