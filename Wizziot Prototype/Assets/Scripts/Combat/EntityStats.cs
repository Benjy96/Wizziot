using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour {

    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    private int currentHealth;

    public int maxStamina = 100;
    public int CurrentStamina { get; private set; }
    private int currentStamina;

    public Dictionary<Stats, Stat> stats;

    public float instantAbilityCost = 10;
    public float aoeAbilityCost = 30;
    public float defenseAbilityCost = 40;

    public float magicKnockbackForce = 0f;
    public float damageModifier = 1f;
    public float mitigateChance = .01f;
    public float damageReduction = .05f;

    [Range(0, 225)] public float sqrMaxTargetDistance;
    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
        //TODO: Set stats according to difficulty if NPC - OR do it from enemy parent class
    }

    private void Start()
    {
        //TODO: Equipment and item stats, modifiers, and SOs
        stats = new Dictionary<Stats, Stat>();

        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
        {
            stats.Add(stat, new Stat(1f));
        }
    }

    //TODO: Make a link method in Enemy.cs to connect abil component and stats - this is really messy
    //TODO: Finish implementing methods to take damage
    public void Damage(Abilities abilityUsed)
    {
        //float amount = 0f;

        //if (GameMetaInfo._Is_Instant_Ability(abilityUsed))
        //{
        //    amount = instantAbilityCost - (instantAbilityCost * damageReduction);
        //}
        //else if (GameMetaInfo._Is_AoE_Ability(abilityUsed))
        //{
        //    amount = aoeAbilityCost * damageModifier;
        //}
        //else if (GameMetaInfo._Is_Defense_Ability(abilityUsed))
        //{
        //    amount = defenseAbilityCost * damageModifier;
        //}

        //if (Random.Range(0f, 1f) < mitigateChance)
        //{
        //    Debug.Log(transform.name + " mitigated the attack");
        //}
        //else
        //{   //Damage reduction reduces damage amount by a % value (itself)
        //    amount = amount - (amount * damageReduction);
        //    CurrentHealth -= Mathf.RoundToInt(amount);
        //    Debug.Log(CurrentHealth);
        //}

        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        //TODO: End game / respawn - use game manager
    }

    public bool CanUseAbility(Abilities abil)
    {
        //if (GameMetaInfo._Is_Instant_Ability(abil))
        //{
        //    float result = CurrentStamina - instantAbilityCost;
        //    if (result >= 0) return true;
        //}
        //else if (GameMetaInfo._Is_AoE_Ability(abil))
        //{
        //    float result = CurrentStamina - aoeAbilityCost;
        //    if (result >= 0) return true;
        //}
        //else if (GameMetaInfo._Is_Defense_Ability(abil))
        //{
        //    float result = CurrentStamina - defenseAbilityCost;
        //    if (result >= 0) return true;
        //}
        return false;
    }
}

// ----- STAT CLASS ----- //
[System.Serializable]
public class Stat
{
    //The stat that this stat "is"
    public Stats stat;
    //Range of 0 to 300%
    [Range(0f, 3f)] public float value;

    public Stat(float baseValue)
    {

    }
}

public enum Stats
{
    AbilityCost_Area, AbilityCost_Defense, AbilityCost_Instant,
    ActionCostReduction, DamageModifier, DamageReduction, MitigationChance, MaxHealthModifier, MaxStaminaModifier,
    SightRange, MovementSpeed
}