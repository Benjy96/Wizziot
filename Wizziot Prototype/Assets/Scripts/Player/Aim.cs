using UnityEngine;

public class Aim : MonoBehaviour {

    private float range;
    private bool gotRangeVal = false;

    void Update () {
        if (gotRangeVal == false)
        {
            range = Mathf.Sqrt(PlayerManager.Instance.playerStats.sqrMaxTargetDistance);
            range /= GameMetaInfo._DIFFICULTY_SCALE;
            gotRangeVal = true;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit pointHit;

        if (Physics.Raycast(ray, out pointHit, range, LayerMask.GetMask("Ground")))
        {
            transform.position = pointHit.point;
        }
    }
}