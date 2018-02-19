using UnityEngine;

public class Targetable : MonoBehaviour {

    //Parent "type" class for all targetable objects - allows player to target objects (distinguish from non-targetable)
    [HideInInspector] public TargetType targetType;  
}

public enum TargetType { Null, Item, Enemy, Story }
