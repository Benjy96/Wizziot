using System;
using System.Collections;
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
    public float instantAbilityCost = 30f;
    public float areaAbilityCost = 50f;
    public float defenseAbilityCost = 40f;
    public Dictionary<Abilities, float> abilityCosts;

    [Tooltip("How much attacks anger the target EmotionChip")][Header("Attack Agro")]
    public float agro = 0.5f;   //Affected by the "Reputation" stat - higher means enemies will be angered more easily

    [Header("Default Stat Modifier Values")]
    public float defaultModifierValue = 1f;
    public Dictionary<Stats, Stat> statModifiers;

    public Action onDeath;

    protected void Awake()
    {
        onDeath += InvokeTargetDestroyedEvent;  //Unsubscribe in OnDisable

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
    }

    protected void OnDisable()
    {
        onDeath -= InvokeTargetDestroyedEvent;
    }

    protected void Update()
    {
        if(CurrentStamina != maxStamina) CurrentStamina = Mathf.Lerp(CurrentStamina, maxStamina, Time.deltaTime / statModifiers[Stats.Fitness].StatValue);
    }

    //Check if target & reset target indicator and invoke onTargetDestroyed if so
    public virtual void InvokeTargetDestroyedEvent()
    {
        Projector playerTarget = GetComponentInChildren<Projector>();
        if(playerTarget != null)
        {
            if(PlayerManager.Instance.onTargetDestroyed != null) PlayerManager.Instance.onTargetDestroyed.Invoke();
        }
    }

    public virtual void ApplyStatModifiers()
    {
        //Scale is "1 + difficulty %" i.e.: Easy = + 0, Normal = + 0.25, Hard = + 0.5, Suicidal = + 0.75. Suicidal modifier would be 1.75
        float difficultyScale = GameMetaInfo._DIFFICULTY_SCALE;
        
        //Set Modifiers
        foreach (KeyValuePair<Stats,Stat> item in statModifiers)
        {
            item.Value.StatValue *= difficultyScale;
        }

        //Apply Modifiers to Variables
        maxHealth *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        maxStamina *= (int)statModifiers[Stats.MaxHealthModifier].StatValue;
        sqrMaxTargetDistance *= statModifiers[Stats.SightRange].StatValue;
        speed *= statModifiers[Stats.MovementSpeed].StatValue;
        turnSpeed *= statModifiers[Stats.MovementSpeed].StatValue;
        agro *= statModifiers[Stats.Reputation].StatValue;

        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    /// <summary>
    /// Interface method for external agents to heal "this"
    /// </summary>
    public void Heal(float amount)
    {
        amount *= statModifiers[Stats.Fitness].StatValue;
        CurrentHealth += (int)amount;
    }

    /// <summary>
    /// Public interface method for external agents to damage "this"
    /// </summary>
    /// <param name="amount">How powerful the attack is. External agent provides this value</param>
    public void Damage(float amount)
    {
        //1. Mitigate Attack
        if(UnityEngine.Random.Range(0, 100) < statModifiers[Stats.MitigationChance].StatValue)  //Max 3% mitigation chance (range: 0 -> 3 for Stat Value)
        {
            Debug.Log("Attack mitigated");
            return;
        }

        //2. Apply damage reduction
        amount /= statModifiers[Stats.DamageReduction].StatValue;   //Max 300% damage reduction (Stat Value: 3)
        
        CurrentHealth -= (int)amount;

        //3. Reduce health
        if (CurrentHealth <= 0)
        {
            if (onDeath != null) onDeath.Invoke();
        }
        else
        {
            Mathf.Clamp(CurrentHealth, 0, maxHealth);
        }
    }

    /// <summary>
    /// Damages the target over time
    /// </summary>
    public IEnumerator DoTDamage(float amount, float duration)
    {
        float damageApplied = 0f;
        float increments = amount / duration;

        while(damageApplied != amount)
        {
            float damage = increments;
            //1. Mitigate Attack
            if (UnityEngine.Random.Range(0, 100) < statModifiers[Stats.MitigationChance].StatValue)
            {
                Debug.Log("Attack mitigated");
                yield break;
            }

            //2. Apply damage reduction
            damage /= statModifiers[Stats.DamageReduction].StatValue;

            CurrentHealth -= (int)damage;

            //3. Reduce health
            if (CurrentHealth <= 0)
            {
                if (onDeath != null) onDeath.Invoke();
            }
            else
            {
                Mathf.Clamp(CurrentHealth, 0, maxHealth);
            }

            damageApplied += damage;
            yield return new WaitForSeconds(1f);    //amount / duration scales to 1s increments
        }
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

    /// <summary>
    /// Calculates damage or heal amount based upon ability costs
    /// </summary>
    private float CalculateAbilDamage(Abilities ability)
    {
        float damage = 0f;
        //Damage dependent upon NPC ability cost, health, and then damage modifier. Increase in health/Decreases in cost will increase the damage the NPC can do.
        if (GameMetaInfo._Is_Instant_Ability(ability)) damage = instantAbilityCost;
        else if (GameMetaInfo._Is_AoE_Ability(ability)) damage = areaAbilityCost;
        else if (GameMetaInfo._Is_Defense_Ability(ability)) damage = defenseAbilityCost;
        damage /= 2;

        damage *= statModifiers[Stats.DamageModifier].StatValue;
        return damage;
    }

    /// <summary>
    /// Calculates ability stamina cost using ability costs and ACR stat modifier
    /// </summary>
    private float GetNewStaminaForUsingAbil(Abilities ability)
    {
        //Calculate Stamina to remove for using the ability
        float staminaToReduceBy = abilityCosts[ability] * statModifiers[Stats.ActionCostReduction].StatValue;
        CurrentStamina -= (int)staminaToReduceBy;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);

        return CurrentStamina;
    }
}