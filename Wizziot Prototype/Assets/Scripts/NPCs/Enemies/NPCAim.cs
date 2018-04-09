using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAim : MonoBehaviour {

    Transform player;

	// Use this for initialization
	void Start () {
        player = PlayerManager.Instance.player.GetComponent<Transform>();
	}
}
