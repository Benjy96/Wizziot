using UnityEngine;

public class EntityStats : MonoBehaviour {

    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    public Stat damageModifier;
    public Stat mitigateChance;
    public Stat damageReduction;

	public void Damage(int amount)
    {
        if(Random.Range(0, 100) > mitigateChance.Value)
        {
            Debug.Log(transform.name + " mitigated the attack");
            return;
        }
        else
        {   //Damage reduction reduces damage amount by a % value (itself)
            amount = ((damageReduction.Value / 100 ) * amount);
            CurrentHealth -= amount;
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
