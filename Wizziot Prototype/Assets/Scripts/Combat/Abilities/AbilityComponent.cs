using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for using Abilities for use by Player & AI.
/// Applies damage & influence using stat component.
/// </summary>
[RequireComponent(typeof(AgentStats))]
public class AbilityComponent : MonoBehaviour {

    public event Action OnPlayerAbilitySelected;
    public event Action OnPlayerAbilityUsed;

    private AgentStats statComponent;
    private AgentStats currentTargetStats;

    //Ability State Data
    [HideInInspector] public Abilities SelectedAbility;
    public List<Abilities> unlockedAbilities;

    private Transform currentTarget;
    private Enemy targetEnemy;

    [Header("Ability Cooldown")]
    public float globalCooldown = 1f;   //Standard cooldown - to prevent too many abil uses, going to use "Stamina" from stat system
    private float globalCooldownFinishTime; //Used to track global cooldown

    [Header("AoE Ability Related")]
    public Transform aimingDisc;
    public float aoeCooldown = 3f;
    private bool aiming = false;
    private Transform instantiatedAimingDisc;
    private float aoeFinishTime;

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
    private float healFXTime = -1f;    //Time before heal after pressed: -1f BECAUSE EFFECT TIME IS INACCURATE - THIS IS TO SYNC THE FX WITH HEALTH INCREASE

    public float GetCurrentCooldown(Abilities abil)
    {
        if (GameMetaInfo._Is_Instant_Ability(abil))
        {
            if (SelectedAbility == Abilities.Confuse) return confuseFinishTime - Time.time;
            else
            {
                Debug.Log("global finish time : " + globalCooldownFinishTime);
                return globalCooldownFinishTime - Time.time;
            }
        }
        else if (GameMetaInfo._Is_Defense_Ability(abil))
        {
            return healFinishTime - Time.time;
        }
        else if (GameMetaInfo._Is_AoE_Ability(abil))
        {
            return aoeFinishTime - Time.time;
        }
        return 0f;
    }

    private void Awake()
    {
        statComponent = GetComponent<AgentStats>();
        if (zapSource != null)
        {
            zapTargeter = zapSource.GetComponentInChildren<particleAttractorLinear>();
            zapParticles = zapSource.GetComponent<ParticleSystem>();
        }

        if (healPrefab != null)
        {
            additionalHealCooldown += healPrefab.GetComponent<ParticleSystem>().main.duration;
            healFXTime += healPrefab.GetComponent<ParticleSystem>().main.duration;
        }
    }

    //Kebyind & UI button accesses this from controller (Start @ 1 to correspond to player UI)
    public void SelectAbility(Abilities choice)
    {
        if (unlockedAbilities.Contains(choice))
        {
            if (!GameMetaInfo._Is_AoE_Ability(choice))
            {
                CancelAoE();
            }
            SelectedAbility = choice;
        }
    }

    /// <summary>
    /// AI Should use this interface method to attack
    /// </summary>
    /// <param name="target">Target if one is selected. Null if not</param>
    /// <returns>Returns true when damage can be applied to the target's stats (once the attack has been executed)</returns>
    public void AIUseSelected(Transform target)
    {
        SetTarget(target);
        UseAbility();
        if (!GameMetaInfo._Is_Defense_Ability(SelectedAbility))
        {
            if(targetEnemy != null) targetEnemy.Influence(gameObject, Emotion.Anger, statComponent.agro);
        }
    }

    //Player interface
    public void PlayerUseInstant(Abilities ability, Transform target)  
    {
        SetTarget(target);

        if (SelectedAbility == ability)
        {
            if (currentTarget != null)
            {
                if(targetEnemy != null)
                {
                    targetEnemy.Influence(gameObject, Emotion.Anger, statComponent.agro);
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
            if (OnPlayerAbilitySelected != null) OnPlayerAbilitySelected.Invoke();
            UseAbility();
        }
    }

    //Player interface
    public void PlayerUseAoE(Abilities ability)    
    {
        SelectAbility(ability);
        if (OnPlayerAbilitySelected != null) OnPlayerAbilitySelected.Invoke();
        UseAbility();
    }

    #region Implementation
    private void SetTarget(Transform target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            currentTargetStats = currentTarget.GetComponent<AgentStats>();
            targetEnemy = target.GetComponent<Enemy>();
        }
    }

    private bool UseAbility()
    {
        float damageToDo = 0f;
        bool aoePlaced = false;

        if (Time.time > globalCooldownFinishTime)
        {
            switch (SelectedAbility)
            {
                case Abilities.Zap:
                    statComponent.TryUseAbility(SelectedAbility, out damageToDo);
                    Zap(damageToDo);
                    break;

                case Abilities.Confuse:
                    if (Time.time > confuseFinishTime)
                    {
                        statComponent.TryUseAbility(SelectedAbility, out damageToDo);
                        Confuse();
                    }
                    break;

                case Abilities.Vortex:
                    if (Time.time > aoeFinishTime)
                    {
                        AoE(vortexPrefab, ref aoePlaced, SelectedAbility);
                        if (aoePlaced)
                        {
                            globalCooldownFinishTime = Time.time + globalCooldown;
                            if (OnPlayerAbilityUsed != null) OnPlayerAbilityUsed.Invoke();
                        }
                    }
                    break;

                case Abilities.Singularity:
                    if (Time.time > aoeFinishTime)
                    {
                        AoE(singularityPrefab, ref aoePlaced, SelectedAbility);
                        if (aoePlaced)
                        {
                            globalCooldownFinishTime = Time.time + globalCooldown;
                            if (OnPlayerAbilityUsed != null) OnPlayerAbilityUsed.Invoke();
                        }
                    }
                    break;

                case Abilities.Heal:
                    if (Time.time > healFinishTime)
                    {
                        statComponent.TryUseAbility(SelectedAbility, out damageToDo);
                        StartCoroutine(Heal(damageToDo));
                        if (OnPlayerAbilityUsed != null) OnPlayerAbilityUsed.Invoke();
                    }
                    break;
            }
            //Add to GCD if not an AoE - AoEs handle cooldown with bool check to verify they have actually been placed
            if (!GameMetaInfo._Is_AoE_Ability(SelectedAbility))
            {
                globalCooldownFinishTime = Time.time + globalCooldown;  //Handle global cooldown
                if (OnPlayerAbilityUsed != null) OnPlayerAbilityUsed.Invoke();
            }
            return true;
        }
        return false;
    }

    private void Zap(float damage)  
    {
        if (currentTarget != null)
        {
            AudioManager.Instance.Play("Zap");
            StartCoroutine(ShotEffect());
            currentTargetStats.Damage(damage);
        }
    }

    private IEnumerator ShotEffect()
    {
        if(zapTargeter.target != currentTarget) zapTargeter.target = currentTarget;
        zapParticles.time = 0;
        zapParticles.Play(true);

        yield return zapEffectDuration;

        zapParticles.Stop(true);
    }

    private void Confuse()
    {
        //Enable effect
        EmotionChip e = currentTarget.GetComponent<EmotionChip>();
        if(e != null)
        {
            Debug.Log("Influencing " + currentTarget.name + " with confuse...");
            e.Influence(Emotion.Fear, 1f);
        }
        GameObject confuseFX = Instantiate(confusePrefab, currentTarget, false);
        AudioManager.Instance.Play("Confuse");
        //Disable effect
        confuseFinishTime = Time.time + confuseDuration;
        Destroy(confuseFX, confuseDuration);
    }

    private void AoE(GameObject spellPrefab, ref bool aoePlaced, Abilities abil)
    {
        AreaAbility deployed = null;

        if (aiming == false)
        {
            aiming = true;
            instantiatedAimingDisc = Instantiate(aimingDisc);
        }
        else
        {
            GameObject areaObject = Instantiate(spellPrefab,
                instantiatedAimingDisc.position + new Vector3(0f, instantiatedAimingDisc.localScale.y / 2, 0f), Quaternion.identity);

            deployed = spellPrefab.GetComponent<AreaAbility>();

            if(instantiatedAimingDisc.gameObject != null) Destroy(instantiatedAimingDisc.gameObject);
            aiming = false;

            float damage = 0f;
            statComponent.TryUseAbility(abil, out damage);
            ApplyAoEEffects(damage, areaObject.transform, deployed);

            aoePlaced = true;
            aoeFinishTime = Time.time + aoeCooldown;
        }
    }

    private void CancelAoE()
    {
        if (instantiatedAimingDisc != null) Destroy(instantiatedAimingDisc.gameObject);
        aiming = false;
    }

    /// <summary>
    /// Gets, damages and influences targets within area of effect
    /// </summary>
    private void ApplyAoEEffects(float damageToDo, Transform objectPosition, AreaAbility AoEUsed)
    {
        List<Enemy> enemies = new List<Enemy>();
        //Influence enemies within AoE sphere
        Collider[] cols = Physics.OverlapSphere(objectPosition.position, 
            AoEUsed.effectRadius); //prefab is scaled down - scale up

        foreach (Collider c in cols)
        {
            if (c.name.Split('(')[0].Equals(AoEUsed.name) || c.tag.Equals(gameObject.tag)) continue;

            Enemy e = c.GetComponentInParent<Enemy>();
            if (e != null) e.Influence(gameObject, Emotion.Anger, statComponent.agro); enemies.Add(e);

            AgentStats eS = c.GetComponentInParent<AgentStats>();
            if (eS != null)
            {
                StartCoroutine(eS.DoTDamage(damageToDo, AoEUsed.duration));
            }
        }
    }

    private IEnumerator Heal(float amount)
    {
        //Means if we change target, the new target isn't healed when coroutine finishes
        AgentStats tempTarget = currentTargetStats;

        if(tempTarget != null)
        {
            healFinishTime = Time.time + additionalHealCooldown;
            GameObject fx = Instantiate(healPrefab, currentTarget, false);
            fx.transform.position = currentTarget.position;
            fx.transform.parent = currentTarget;

            yield return new WaitForSeconds(healFXTime);
            AudioManager.Instance.Play("Heal");
            tempTarget.Heal(amount);

            Destroy(fx);
        }
        else
        {
            Debug.Log("Heal on CD");
        }
    }
    #endregion
}