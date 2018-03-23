using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour {

    [Header("Standard Stats")]
    public int maxHealth = 100;
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    private int currentHealth;

    public int maxStamina = 100;
    public float CurrentStamina { get { return currentStamina; } private set { currentStamina = value; } }
    private float currentStamina;

    [Range(0, 225)] public float sqrMaxTargetDistance;
    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;

    [Header("Base Cost of Abilities")]
    public float instantAbilityCost = 10f;
    public float areaAbilityCost = 30f;
    public float defenseAbilityCost = 40f;
    public Dictionary<Abilities, float> abilityCosts;

    [Header("Default Stat Modifier Values")]
    public float defaultModifierValue = 1f;
    public Dictionary<Stats, Stat> statModifiers;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;

        //Set ability costs
        abilityCosts = new Dictionary<Abilities, float>();
        foreach (Abilities ability in Enum.GetValues(typeof(Abilities)))
        {
            if (GameMetaInfo._Is_Instant_Ability(ability))
            {
                abilityCosts.Add(ability, instantAbilityCost);
            }
            else if (GameMetaInfo._Is_AoE_Ability(ability))
            {
                abilityCosts.Add(ability, areaAbilityCost);
            }
            else if (GameMetaInfo._Is_Defense_Ability(ability))
            {
                abilityCosts.Add(ability, areaAbilityCost);
            }
        }

        //Initialise Stat Modifiers and Values - Stats Enum as key, Stat Class with type (Stats) and value - Default val: 1
        statModifiers = new Dictionary<Stats, Stat>();
        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
        {
            Stat newStat = new Stat(stat, defaultModifierValue);
            statModifiers.Add(newStat.StatType, newStat);
        }

        ApplyStatModifiers();
    }

    private void Update()
    {
        if(CurrentStamina != maxStamina) CurrentStamina = Mathf.Lerp(CurrentStamina, maxStamina, Time.deltaTime / statModifiers[Stats.Fitness].StatValue);
    }

    //TODO: Use difficulty setting to modify
    public void ApplyStatModifiers()
    {
        //Apply Modifiers
        maxHealth *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        maxStamina *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;

        sqrMaxTargetDistance *= statModifiers[Stats.SightRange].StatValue;

        speed *= statModifiers[Stats.MovementSpeed].StatValue;
        turnSpeed *= statModifiers[Stats.MovementSpeed].StatValue;
    }

    /// <summary>
    /// Public interface method for external agents to damage "this"
    /// </summary>
    /// <param name="amount">How powerful the attack is. External agent provides this value</param>
    public void Damage(float amount)
    {
        //1. Mitigate Attack
        if(UnityEngine.Random.Range(0, 100) < statModifiers[Stats.MitigationChance].StatValue)
        {
            Debug.Log("Attack mitigated");
            return;
        }

        //2. Apply damage reduction
        amount *= statModifiers[Stats.DamageReduction].StatValue;

        CurrentHealth -= (int)amount;

        //3. Reduce health
        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            Mathf.Clamp(CurrentHealth, 0, maxHealth);
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        //TODO: End game / respawn - use game manager
        //TODO: Object pool AI
    }

    /// <summary>
    /// Used by self (controller or abil component) to attack other. Will apply modifiers and return a damage amount to apply to other entity.
    /// </summary>
    /// <returns>True if you have enough stamina</returns>
    public bool TryUseAbility(Abilities ability, out float damage)
    {
        if (abilityCosts[ability] <= CurrentStamina)
        {
            CurrentStamina = GetNewStaminaForUsingAbil(ability);
            damage = CalculateAbilDamage(ability);
            return true;
        }

        damage = 0f;
        return false;
    }

    private float CalculateAbilDamage(Abilities ability)
    {
        float damage = 0f;
        //Damage dependent upon NPC ability cost, health, and then damage modifier. Increase in health/Decreases in cost will increase the damage the NPC can do.
        if (GameMetaInfo._Is_Instant_Ability(ability)) damage = maxHealth - instantAbilityCost;
        else if (GameMetaInfo._Is_AoE_Ability(ability)) damage = maxHealth - areaAbilityCost;
        else if (GameMetaInfo._Is_Defense_Ability(ability)) damage = maxHealth - defenseAbilityCost;
        damage /= 2;

        damage *= statModifiers[Stats.DamageModifier].StatValue;
        return damage;
    }

    private float GetNewStaminaForUsingAbil(Abilities ability)
    {
        //Calculate Stamina to remove for using the ability
        float staminaToReduceBy = abilityCosts[ability] * statModifiers[Stats.ActionCostReduction].StatValue;
        CurrentStamina -= (int)staminaToReduceBy;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);

        return CurrentStamina;
    }
}