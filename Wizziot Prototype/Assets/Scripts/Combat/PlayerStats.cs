using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats {

    private void Start()
    {
        ApplyStatModifiers();
    }

    public override void ApplyStatModifiers()
    {
        base.ApplyStatModifiers();

        //TODO: attack agro adjusted by difficulty
    }

    public override void InvokeTargetDestroyedEvent()
    {
        base.InvokeTargetDestroyedEvent();
    }
}
