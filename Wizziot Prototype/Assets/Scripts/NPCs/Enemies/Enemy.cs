using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Targetable {

    protected static PlayerController player;

	protected void Awake ()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
	}
}
