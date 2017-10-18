using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    protected static PlayerController player;
    protected static bool externalsSet = false;

    public Script story;
    public string inkPath = "";

    public abstract void Interact();
    public abstract void SetExternalFunctions();
}
