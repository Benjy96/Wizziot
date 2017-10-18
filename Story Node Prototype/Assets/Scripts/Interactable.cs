using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    protected static PlayerController player;
    protected static bool externalsSet = false;

    public Script story;
    public string inkPath = "";
    
    protected float pushOffForce = 20f;

    public abstract void Interact();
    public abstract void SetExternalFunctions();
}
