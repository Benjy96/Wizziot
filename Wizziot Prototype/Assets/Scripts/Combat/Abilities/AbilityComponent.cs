using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Abil cost/dmg variables
    //Needs linked to entity stats - which stored where?
    //Stats relate to the entity - abil dmg / costs should be stored here
        //BUT stats can then modify the values from here
public class AbilityComponent : MonoBehaviour {

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
    public float confuseDuration = 3f;
    private float confuseFinishTime;    //Used to track when confuse debuff will wear off - needs separate cooldown tracker (other than global)

    [Header("Vortex")]
    public GameObject vortexPrefab;

    [Header("Singularity")]
    public GameObject singularityPrefab;

    private void Awake()
    {
        zapTargeter = zapSource.GetComponentInChildren<particleAttractorLinear>();
        zapParticles = zapSource.GetComponent<ParticleSystem>();
    }

    private void SetTarget(Transform target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            targetRB = target.GetComponent<Rigidbody>();
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
    public bool UseSelected(Transform target)
    {
        SetTarget(target);
        return UseAbility();
    }

    //Player interface
    public void UseInstantAbility(Abilities ability, Transform target)  //TODO; make bool for player?
    {
        SetTarget(target);

        if (SelectedAbility == ability)
        {
            if (currentTarget != null)
            {
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
    public void UseAOEAbility(Abilities ability)    //TODO: make bool for player?
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
    private bool UseAbility()
    {
        if (Time.time > globalCooldownFinishTime)
        {
            switch (SelectedAbility)
            {
                case Abilities.Zap:
                    Zap();
                    break;

                case Abilities.Confuse:
                    StartCoroutine(Confuse());
                    break;

                case Abilities.Vortex:
                    AoE(vortexPrefab);
                    break;

                case Abilities.Singularity:
                    AoE(singularityPrefab);
                    break;
            }
            globalCooldownFinishTime = Time.time + globalCooldown;
            return true;
        }
        else
        {
            Debug.Log("On cooldown...");
            return false;
        }
    }

    private void Zap()  
    {
        if (currentTarget != null)
        {
            StartCoroutine(ShotEffect());
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
                Debug.Log("Influencing with confuse...");
                e.Influence(Emotion.Fear, .5f); //TODO: Where to set strength of influence? Stats? Factor of powers? Level?
            }
            yield return new WaitForSeconds(confuseDuration);
            //Disable effect
            confuseFinishTime = Time.time + confuseDuration;
        }
        else
        {
            Debug.Log("Target is already confused!");
        }
    }

    private void AoE(GameObject spellPrefab)
    {
        if (aiming == false)
        {
            aiming = true;
            instantiatedAimingDisc = Instantiate(aimingDisc);
        }
        else
        {
            Instantiate(spellPrefab,
                instantiatedAimingDisc.position + new Vector3(0f, instantiatedAimingDisc.localScale.y / 2, 0f), Quaternion.identity);
            Destroy(instantiatedAimingDisc.gameObject);
            aiming = false;
        }
    }
    #endregion
}