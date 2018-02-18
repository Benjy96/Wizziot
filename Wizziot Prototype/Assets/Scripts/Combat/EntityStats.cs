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

    public Dictionary<Stats, Stat> entityStats;

    [Range(0, 225)] public float sqrMaxTargetDistance;
    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;

        entityStats = new Dictionary<Stats, Stat>();

        //Create a Stat for every Stat type
        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
        {
            Stat newStat = new Stat(stat, 1f);
            entityStats.Add(newStat.StatType, newStat);
        }
    }

    private void Start()
    {
        //TODO: Apply differing stats depending on difficulty
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
        //TODO: Object pool AI
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
    ActionCostReduction, DamageModifier, DamageReduction,  MaxHealthModifier, MaxStaminaModifier, MitigationChance, SightRange, MovementSpeed
}