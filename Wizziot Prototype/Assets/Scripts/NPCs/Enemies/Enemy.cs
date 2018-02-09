using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: AI Code <- Use enemy as parent / controller for behaviour (use a motor component and ability component - this class is the brain)
//TODO: Create movement component
//TODO: Create NPC aim component (e.g. track towards player with varying speed/accuracy depending on difficult)

public class Enemy : Targetable {

    protected static PlayerController player;

	protected void Awake ()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
	}
}
