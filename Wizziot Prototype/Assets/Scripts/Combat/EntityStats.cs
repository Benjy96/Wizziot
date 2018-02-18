using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour {

    public int maxHealth = 100;
    public int CurrentHealth { get { return currentHealth; } private set { currentHealth = value; } }
    private int currentHealth;

    public int maxStamina = 100;
    public int CurrentStamina { get { return currentStamina; } private set { currentStamina = value; } }
    private int currentStamina;

    public List<Stat> entityStats;

    public Dictionary<Stats, Stat> statModifiers;
    public float baseStatModifier = 1f;

    [Range(0, 225)] public float sqrMaxTargetDistance;
    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;

        statModifiers = new Dictionary<Stats, Stat>();

        //Create a Stat for every Stat type
        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
        {
            Stat newStat = new Stat(stat, baseStatModifier);
            statModifiers.Add(newStat.StatType, newStat);
        }

        ApplyStatModifiers();
    }

    //TODO: How can we remove the explicitness of this programming?
    //Need a way to store the variables and modifiers
    //Perhaps an enum of Stats and StatsModifiers
        //Prob is not every stat applies to everything - would need to be mapped
    private void ApplyStatModifiers()
    {
        //See Stat.cs - can loop through enums instead of explicit

        //Apply Modifiers
        maxHealth *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        maxStamina *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        sqrMaxTargetDistance *= statModifiers[Stats.SightRange].StatValue;
        speed *= statModifiers[Stats.MovementSpeed].StatValue;
        turnSpeed *= statModifiers[Stats.MovementSpeed].StatValue;
    }

    private void Start()
    {
        //TODO: Apply differing stats depending on difficulty
    }

    /// <summary>
    /// Public interface method for external agents to damage "this"
    /// </summary>
    /// <param name="amount">How powerful the attack is. External agent provides this value</param>
    public void Damage(float amount)
    {
        //Agent has calculated their own damage from abil component and stats (determined by modifiers)
        //Now apply own stats to modifying the amount

        //1. Apply dmg reduction stats to amount
        //2. Reduce health

        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        //TODO: End game / respawn - use game manager
        //TODO: Object pool AI
    }

    /// <summary>
    /// Used by self (controller or abil component) to attack other
    /// </summary>
    /// <returns></returns>
    public bool UseAbility(Abilities ability)
    {
        return false;
    }
}