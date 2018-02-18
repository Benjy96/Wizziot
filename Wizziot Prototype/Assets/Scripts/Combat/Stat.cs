using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private Stats stat; //The stat that this stat "is"
    [SerializeField, Range(0f, 3f)] private float value;    //Range of 0 to 300%

    public Stats StatType { get { return stat; } }
    public float StatValue { get { return value; } }

    public Stat(Stats stat, float baseValue)
    {
        this.stat = stat;
        value = baseValue;
    }
}

public enum Stats
{
    ActionCostReduction, DamageModifier, DamageReduction, MaxHealthModifier, MaxStaminaModifier, MitigationChance, SightRange, MovementSpeed
}
