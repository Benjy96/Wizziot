using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    protected static PlayerController player;
    protected static Script story;

    public string inkPath = "";

    public abstract void Interact();
    public abstract void SetExternalFunctions();

    protected void Awake()
    {
        if (story == null) story = FindObjectOfType<Script>();
        if (player == null) player = FindObjectOfType<PlayerController>();
    }
}
