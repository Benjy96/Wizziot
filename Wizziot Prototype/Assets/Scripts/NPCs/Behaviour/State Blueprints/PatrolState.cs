using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State {

    protected override State EnterState(Enemy owner)
    {
        return base.EnterState(owner);
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
