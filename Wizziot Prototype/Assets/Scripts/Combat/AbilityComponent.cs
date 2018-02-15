using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityComponent : MonoBehaviour {

    //Ability State Data
    [HideInInspector] public Abilities SelectedAbility;
    public List<Abilities> unlockedAbilities;

    private string hostileTagName;

    [Header("Ability Cooldown")]
    public float globalCooldown = 1f;   //Standard cooldown - to prevent too many abil uses, going to use "Stamina" from stat system
    private float globalCooldownFinishTime; //Used to track global cooldown

    [Header("AoE Ability Related")]
    public Transform aimingDisc;
    private bool aiming = false;
    private Transform instantiatedAimingDisc;

    [Header("Zap")]
    public GameObject zapSource;
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
    }

    //Kebyind & UI button accesses this from controller (Start @ 1 to correspond to player UI)
    public void SelectAbility(Abilities choice)
    {
        if (unlockedAbilities.Contains(choice))
        {
            SelectedAbility = choice;
        }
    }

    //AI Interface
    public void UseSelected(Transform target)
    {
        if (GameMetaInfo._Is_Instant_Ability(SelectedAbility))
        {
            UseInstantAbility(SelectedAbility, target);
        }
        else if (GameMetaInfo._Is_AoE_Ability(SelectedAbility))
        {
            UseAOEAbility(SelectedAbility);
        }
        else if (GameMetaInfo._Is_Defense_Ability(SelectedAbility))
        {
            UseInstantAbility(SelectedAbility, target);
        }
    }

    //Player interface
    public void UseInstantAbility(Abilities ability, Transform target)
    {
        if (SelectedAbility == ability)
        {
            if (target != null)
            {
                UseAbility(target.transform);
            }
            else
            {
                Debug.Log("Invalid target: " + target.name);
            }
        }
        else
        {
            SelectAbility(ability);
        }
    }

    //Player interface
    public void UseAOEAbility(Abilities ability)
    {
        if (SelectedAbility == ability)
        {
            UseAbility(null);
        }
        else
        {
            SelectAbility(ability);
        }
    }

    #region Implementation
    private void UseAbility(Transform target)
    {
        if (Time.time > globalCooldownFinishTime)
        {
            switch (SelectedAbility)
            {
                case Abilities.Zap:
                    Zap(target);
                    break;

                case Abilities.Confuse:
                    StartCoroutine(Confuse(target));
                    break;

                case Abilities.Vortex:
                    AoE(vortexPrefab);
                    break;

                case Abilities.Singularity:
                    AoE(singularityPrefab);
                    break;
            }
            globalCooldownFinishTime = Time.time + globalCooldown;
        }
        else
        {
            Debug.Log("On cooldown...");
        }
    }

    private void Zap(Transform target)  
    {
        if (target != null)
        {
            StartCoroutine(ShotEffect(target));
        }
    }

    private IEnumerator ShotEffect(Transform target)
    {
        zapSource.gameObject.SetActive(true);
        zapTargeter.target = target;
        yield return zapEffectDuration;
        zapTargeter.target = null;
        zapSource.gameObject.SetActive(false);
    }

    private IEnumerator Confuse(Transform target)
    {
        //Check if previous confuse debuff has worn off
        if (Time.time > confuseFinishTime)
        {
            //Enable effect
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