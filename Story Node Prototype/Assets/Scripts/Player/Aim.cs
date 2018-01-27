using UnityEngine;

public class Aim : MonoBehaviour {

    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit pointHit;

        if (Physics.Raycast(ray, out pointHit, 100f, LayerMask.GetMask("Ground")))
        {
            transform.position = pointHit.point;
        }
    }
}
