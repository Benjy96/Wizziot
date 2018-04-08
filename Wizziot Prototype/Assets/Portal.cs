using UnityEngine;

public class Portal : MonoBehaviour {

    public Portal destination;

    private void Start()
    {
        if (destination == null) Debug.Log(name + " Portal destination not set");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //Place player outside of collider, towards the direction the portal faces
            Vector3 forward = destination.transform.forward;
            forward *= 5f;
            other.transform.position = destination.transform.position + forward;

            Quaternion x = destination.transform.rotation;
            other.transform.localRotation = x;
        }
    }
}
