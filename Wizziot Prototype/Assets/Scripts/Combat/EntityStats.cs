using UnityEngine;

public class EntityStats : MonoBehaviour {

    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    public int maxStamina = 100;
    public int CurrentStamina { get; private set; }

    [Range(0, 10)] public float speed;
    [Range(50, 150)] public float turnSpeed;
    [Range(0, 225)] public float sqrMaxTargetDistance;

    public float damageModifier;
    public float mitigateChance;
    public float damageReduction;

    public void Damage(float amount)
    {
        if(Random.Range(0, 100) > mitigateChance)
        {
            Debug.Log(transform.name + " mitigated the attack");
            return;
        }
        else
        {   //Damage reduction reduces damage amount by a % value (itself)
            amount = ((damageReduction / 100f ) * amount);
            CurrentHealth -= Mathf.RoundToInt(amount);
        }

        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {

    }
}
