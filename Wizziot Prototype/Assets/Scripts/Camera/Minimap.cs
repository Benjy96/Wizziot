using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    private void LateUpdate()
    {
        Vector3 newPos = PlayerManager.Instance.player.transform.position;
        newPos.y = PlayerManager.Instance.player.transform.position.y + 100f;
        transform.position = newPos;
    }
}
