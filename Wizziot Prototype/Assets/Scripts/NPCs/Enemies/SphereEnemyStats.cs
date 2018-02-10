using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereEnemyStats : EntityStats {

    public override void Die()
    {
        base.Die();

        //TODO: Instantiate loot / insert death animation

        Destroy(gameObject);
    }
}
