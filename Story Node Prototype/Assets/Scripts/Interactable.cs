using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    public Script story;
    public string inkPath = "";
    protected static bool externalsSet = false;
    protected float pushOffForce = 20f;

    public abstract void Interact();
    public abstract void SetExternalFunctions();
}
