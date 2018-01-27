using UnityEngine;

public class Aim : MonoBehaviour {

    //TODO: Consider using something that throws objects at a raycasted point? Actual objects? 
        //Instead of a line renderer?
    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit pointHit;

        if (Physics.Raycast(ray, out pointHit, 100f, LayerMask.GetMask("Ground")))
        {
            transform.position = pointHit.point;
        }
    }
}
