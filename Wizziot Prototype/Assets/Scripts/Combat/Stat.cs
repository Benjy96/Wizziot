using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private Stats stat; //The stat that this stat "is"
    [SerializeField, Range(0f, 3f)] private float statValue;    //Range of 0 to 300%

    public Stats StatType { get { return stat; } }
    public float StatValue { get { return statValue; } set { statValue = Mathf.Clamp(value, 0f, 3f); } }

    public Stat(Stats stat, float baseValue)
    {
        this.stat = stat;
        statValue = baseValue;
    }
}

/// <summary>
/// Not obvious Stats descriptions:
/// ActionCostReduction: Used to calculate ability costs.
/// Fitness: Used to modify heals received & Stamina regeneration rate.
/// MitigationChance: Chance to take no damage at all.
/// Notoriety: Agro modifier (Reputation with NPCs)
/// </summary>
public enum Stats
{
    ActionCostReduction, DamageModifier, DamageReduction, Fitness, MaxHealthModifier, MaxStaminaModifier, MitigationChance, MovementSpeed, Notoriety, SightRange
}
