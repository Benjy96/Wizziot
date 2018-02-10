using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats {

    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;

    public override void Die()
    {
        base.Die();
    }
}
