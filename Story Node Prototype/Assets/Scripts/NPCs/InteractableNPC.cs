using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    // ----- FIELDS ----- //
    protected static PlayerController player;
    protected static Script story;
    [SerializeField] protected string inkPath = "";

    protected Vector3 InteractingNPC
    {
        get
        {
            return player.TargetPos;
        }
    }

    // ----- ABSTRACT METHODS ----- //
    public abstract void Interact();

    protected abstract void SetExternalFunctions();

    // ----- METHODS ----- //
    protected void Awake()
    {
        if (story == null) story = FindObjectOfType<Script>();
        if (player == null) player = FindObjectOfType<PlayerController>();
    }
}
