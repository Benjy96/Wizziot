using System;
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
        ResetModifiers();

        //Apply Modifiers to Variables
        maxHealth *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        maxStamina *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        sqrMaxTargetDistance *= statModifiers[Stats.SightRange].StatValue;
        speed *= statModifiers[Stats.MovementSpeed].StatValue;
        turnSpeed *= statModifiers[Stats.MovementSpeed].StatValue;
        agro *= statModifiers[Stats.Notoriety].StatValue;

        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    public override void InvokeDeathEvent()
    {
        GameManager.Instance.ExitToMenu();
    }
}
