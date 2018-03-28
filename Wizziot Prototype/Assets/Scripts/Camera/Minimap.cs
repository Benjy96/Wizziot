using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    private void LateUpdate()
    {
        Vector3 newPos = PlayerManager.Instance.player.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
