﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Suicide State", menuName = "States/Suicide")]
public class SuicideState : State {

    protected override State EnterState(Enemy owner)
    {
        return base.EnterState(owner);
    }

    public override void Execute()
    {
        ExitState();
        Destroy(owner.gameObject);
    }

    public override void ExitState()
    {
        foreach (Enemy e in owner.neighbourhoodTracker.neighbours)
        {
            e.neighbourhoodTracker.neighbours.Remove(owner);
        }
    }
}
