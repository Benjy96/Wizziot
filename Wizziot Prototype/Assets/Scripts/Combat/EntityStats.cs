using UnityEngine;

public class EntityStats : MonoBehaviour {

    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    private int currentHealth;

    public int maxStamina = 100;
    public int CurrentStamina { get; private set; }
    private int currentStamina;

    public int instantAbilityCost = 10;
    public int aoeAbilityCost = 30;
    public int defenseAbilityCost = 40;

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
    }

    //TODO: Make a link method in Enemy.cs to connect abil component and stats - this is really messy
    public bool Damage(Abilities abilityUsed)
    {
        float amount = 0f;

        if (GameMetaInfo._Is_Instant_Ability(abilityUsed))
        {
            if (CanUseAbility(abilityUsed))
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina -= instantAbilityCost, 0, maxStamina);
                amount = instantAbilityCost * damageModifier;
            }
        }
        else if (GameMetaInfo._Is_AoE_Ability(abilityUsed))
        {
            if (CanUseAbility(abilityUsed))
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina -= aoeAbilityCost, 0, maxStamina);
            }
        }
        else if (GameMetaInfo._Is_Defense_Ability(abilityUsed))
        {
            if (CanUseAbility(abilityUsed))
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina -= defenseAbilityCost, 0, maxStamina);
            }
        }
        else
        {
            return false;
        }

        if (Random.Range(0f, 1f) > mitigateChance)
        {
            Debug.Log(transform.name + " mitigated the attack");
        }
        else
        {   //Damage reduction reduces damage amount by a % value (itself)
            amount = amount - (amount * damageReduction);
            CurrentHealth -= Mathf.RoundToInt(amount);
        }

        if(CurrentHealth <= 0)
        {
            Die();
        }

        return true;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    private bool CanUseAbility(Abilities abil)
    {
        if (GameMetaInfo._Is_Instant_Ability(abil))
        {
            float result = CurrentStamina - instantAbilityCost;
            if (result >= 0) return true;
        }
        else if (GameMetaInfo._Is_AoE_Ability(abil))
        {
            float result = CurrentStamina - aoeAbilityCost;
            if (result >= 0) return true;
        }
        else if (GameMetaInfo._Is_Defense_Ability(abil))
        {
            float result = CurrentStamina - defenseAbilityCost;
            if (result >= 0) return true;
        }
        return false;
    }
}