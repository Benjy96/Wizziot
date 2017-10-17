using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    public Script story;
    protected string inkPath = "";

    public abstract void Interact();
}
