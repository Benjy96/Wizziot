using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for using Abilities for use by Player & AI.
/// Applies damage & influence using stat component.
/// </summary>
[RequireComponent(typeof(EntityStats))]
public class AbilityComponent : MonoBehaviour {

    private EntityStats statComponent;
    private EntityStats currentTargetStats;

    //Ability State Data
    [HideInInspector] public Abilities SelectedAbility;
    public List<Abilities> unlockedAbilities;

    private Transform currentTarget;
    private Rigidbody targetRB;

    [Header("Ability Cooldown")]
    public float globalCooldown = 1f;   //Standard cooldown - to prevent too many abil uses, going to use "Stamina" from stat system
    private float globalCooldownFinishTime; //Used to track global cooldown

    [Header("AoE Ability Related")]
    public Transform aimingDisc;
    private bool aiming = false;
    private Transform instantiatedAimingDisc;

    private float knockbackForce = 0f;
    public float KnockbackForce { get { return knockbackForce; } set { knockbackForce = value; } }

    [Header("Zap")]
    public GameObject zapSource;
    private ParticleSystem zapParticles;
    private particleAttractorLinear zapTargeter;
    private WaitForSeconds zapEffectDuration = new WaitForSeconds(.5f);

    [Header("Confuse")]
    public GameObject confusePrefab;
    public float confuseDuration = 3f;
    private float confuseFinishTime;    //Used to track when confuse debuff will wear off - needs separate cooldown tracker (other than global)

    [Header("Vortex")]
    public GameObject vortexPrefab;

    [Header("Singularity")]
    public GameObject singularityPrefab;

    [Header("Heal")]
    public GameObject healPrefab;
    [Tooltip("Adds onto an existing 5s CD")] public float additionalHealCooldown;
    private float healFinishTime;
    private float healFXTime;

    private void Awake()
    {
        statComponent = GetComponent<EntityStats>();
        if (zapSource != null)
        {
            zapTargeter = zapSource.GetComponentInChildren<particleAttractorLinear>();
            zapParticles = zapSource.GetComponent<ParticleSystem>();
        }

        if (healPrefab != null)
        {
            additionalHealCooldown += healPrefab.GetComponent<ParticleSystem>().main.duration;
            healFXTime = healPrefab.GetComponent<ParticleSystem>().main.duration;
        }
    }

    //Kebyind & UI button accesses this from controller (Start @ 1 to correspond to player UI)
    public void SelectAbility(Abilities choice)
    {
        if (unlockedAbilities.Contains(choice))
        {
            SelectedAbility = choice;
        }
    }

    /// <summary>
    /// AI Should use this interface method to attack
    /// </summary>
    /// <param name="target">Target if one is selected. Null if not</param>
    /// <returns>Returns true when damage can be applied to the target's stats (once the attack has been executed)</returns>
    public void UseSelected(Transform target)
    {
        SetTarget(target);
        UseAbility();
    }

    //Player interface
    public void PlayerUseInstant(Abilities ability, Transform target)  //TODO; make bool for player?
    {
        SetTarget(target);

        if (SelectedAbility == ability)
        {
            if (currentTarget != null)
            {
                Enemy e = currentTarget.GetComponent<Enemy>();
                if(e != null)
                {
                    e.Influence(gameObject, Emotion.Anger, statComponent.attackInfluence);
                }
                UseAbility();
            }
            else
            {
                Debug.Log("Invalid target: " + currentTarget.name);
            }
        }
        else
        {
            SelectAbility(ability);
        }
    }

    //Player interface
    public void PlayerUseAoE(Abilities ability)    //TODO: make bool for player?
    {
        if (SelectedAbility == ability)
        {
            UseAbility();
        }
        else
        {
            SelectAbility(ability);
        }
    }

    #region Implementation
    private void SetTarget(Transform target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            currentTargetStats = currentTarget.GetComponent<EntityStats>();
            targetRB = target.GetComponent<Rigidbody>();
        }
    }

    private bool UseAbility()
    {
        float damageToDo = 0f;
        bool aoePlaced = false;

        if (Time.time > globalCooldownFinishTime)
        {
            if (statComponent.TryUseAbility(SelectedAbility, out damageToDo))
            {
                switch (SelectedAbility)
                {
                    case Abilities.Zap:
                        Zap(damageToDo);
                        break;

                    case Abilities.Confuse:
                        StartCoroutine(Confuse());
                        break;

                    case Abilities.Vortex:
                        AoE(vortexPrefab, damageToDo, ref aoePlaced);
                        if (aoePlaced) globalCooldownFinishTime = Time.time + globalCooldown;
                        break;

                    case Abilities.Singularity:
                        AoE(singularityPrefab, damageToDo, ref aoePlaced);
                        if(aoePlaced) globalCooldownFinishTime = Time.time + globalCooldown;
                        break;

                    case Abilities.Heal:
                        StartCoroutine(Heal(damageToDo));
                        break;
                }
                //Add to GCD if not an AoE - AoEs handle cooldown with bool check to verify they have actually been placed
                if(!GameMetaInfo._Is_AoE_Ability(SelectedAbility)) globalCooldownFinishTime = Time.time + globalCooldown;  //Handle global cooldown
                return true;
            }
            else
            {
                Debug.Log("Not enough stamina");
            }
        }
        return false;
    }

    private void Zap(float damage)  
    {
        if (currentTarget != null)
        {
            StartCoroutine(ShotEffect());
            currentTargetStats.Damage(damage);
        }
    }

    private IEnumerator ShotEffect()
    {
        if(zapTargeter.target != currentTarget) zapTargeter.target = currentTarget;
        zapParticles.time = 0;
        zapParticles.Play(true);
        if (targetRB != null && KnockbackForce > 0)
        {
            targetRB.AddForce(currentTarget.position - transform.position * KnockbackForce);
        }

        yield return zapEffectDuration;

        zapParticles.Stop(true);
    }

    private IEnumerator Confuse()
    {
        //Check if previous confuse debuff has worn off
        if (Time.time > confuseFinishTime)
        {
            //Enable effect
            EmotionChip e = currentTarget.GetComponent<EmotionChip>();
            if(e != null)
            {
                Debug.Log("Influencing " + currentTarget.name + " with confuse...");
                e.Influence(Emotion.Fear, 1f);
            }
            GameObject confuseFX = Instantiate(confusePrefab, currentTarget, false);
            //Disable effect
            confuseFinishTime = Time.time + confuseDuration;
            yield return new WaitForSeconds(confuseDuration);
            Destroy(confuseFX);
        }
        else
        {
            Debug.Log("Confuse not ready!");
        }
    }

    private void AoE(GameObject spellPrefab, float damage, ref bool aoePlaced)
    {
        AreaAbility deployed = null;

        if (aiming == false)
        {
            aiming = true;
            instantiatedAimingDisc = Instantiate(aimingDisc);
        }
        else
        {
            Instantiate(spellPrefab,
                instantiatedAimingDisc.position + new Vector3(0f, instantiatedAimingDisc.localScale.y / 2, 0f), Quaternion.identity);

            deployed = spellPrefab.GetComponent<AreaAbility>();

            Destroy(instantiatedAimingDisc.gameObject);
            aiming = false;

            ApplyAoEEffects(damage, deployed);
            aoePlaced = true;
        }
    }

    /// <summary>
    /// Damage and influence targets within area of effect
    /// </summary>
    /// <param name="damageToDo"></param>
    private void ApplyAoEEffects(float damageToDo, AreaAbility AoEUsed)
    {
        List<Enemy> enemies = new List<Enemy>();
        //Influence enemies within AoE sphere
        Collider[] cols = Physics.OverlapSphere(AoEUsed.transform.position, AoEUsed.effectRadius);
        foreach (Collider c in cols)
        {
            Debug.Log(c.name);
            Enemy e = c.GetComponent<Enemy>();
            if (e != null) e.Influence(gameObject, Emotion.Anger, statComponent.attackInfluence); enemies.Add(e);

            EntityStats eS = c.GetComponent<EntityStats>();
            if (eS != null) StartCoroutine(eS.DoTDamage(damageToDo, AoEUsed.duration));
        }
    }

    private IEnumerator Heal(float amount)
    {
        if(currentTarget != null && Time.time > healFinishTime)
        {
            healFinishTime = Time.time + additionalHealCooldown;
            GameObject fx = Instantiate(healPrefab, currentTarget, false);

            yield return new WaitForSeconds(healFXTime);
            Debug.Log("Healing");
            currentTargetStats.Heal(amount);

            Destroy(fx);
        }
        else
        {
            Debug.Log("Heal on CD");
        }
    }
    #endregion
}