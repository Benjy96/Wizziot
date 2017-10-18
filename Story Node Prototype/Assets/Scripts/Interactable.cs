using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour {

    protected static PlayerController player;
    protected static bool externalsSet = false; 
    //TODO: Find a different way of setting externals - potential problem: If we had two classes that inherit from interactableNPC, only one type's external will be set
    //TODO: Investigate externals - I foresee two different ways: Set externals in the main Script script, or be able to check if an external function is set:
                                                                                                          //This will be either a property of Story, or a bool for each subclass
    //TODO: Check out Story.HasFunction() - I believe this may remove the need for static bools - we can instead check on awake if the function is already assigned,
    //therefore allowing subclasses to inherit SetExternalFunctions without having a torrent of static booleans

    public Script story;
    public string inkPath = "";

    public abstract void Interact();
    public abstract void SetExternalFunctions();
}
