using UnityEngine;

public class PlayerStats : EntityStats {

    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;
    [Range(0, 225)] public float sqrMaxTargetDistance;
}
