using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Restrict spell usage based on durations - e.g. need a cooldown for confuse at least confuse duration? Maybe only for single enemies?
//TODO: Fix potential bug - if object destroyed while AoE trying to manipulate it, it gives an error
    //Maybe just use constantly updating neighbourhood - check boids example
public class AbilityComponent : MonoBehaviour {

    //Ability State Data
    [HideInInspector] public Abilities SelectedAbility;
    public List<Abilities> unlockedAbilities;

    //AoE Variables
    public Transform aimingDisc;
    private bool aiming = false;
    private Transform instantiatedAimingDisc;

    //ZAP REQUIRED VARIABLES
    public float fireRate = .25f;
    public GameObject zapSource;
    private particleAttractorLinear zapTargeter;
    private WaitForSeconds spellDuration = new WaitForSeconds(1f);
    private float nextFire; //track time passed

    //CONFUSE VARIABLES
    public float confuseDuration = 3f;

    //VORTEX VARIABLES
    public GameObject vortexPrefab;

    //SINGULARITY VARIABLES
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
            Debug.Log("Set selected ability to: " + SelectedAbility);
        }
    }

    //Player Calls this from controller - abstracted so NPCs can use!! (UseAbility is a function)
	public void UseAbility(Transform target)
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
    }

#region Zap Implementation
    private void Zap(Transform target)  //TODO: could make this (and all abils) components - add them to player when u unlock the abil (and bind it to a key?)
    {
        if (target != null) //TODO: make so can't shoot story NPCs 
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                StartCoroutine(ShotEffect(target));

                //Destroy(target.gameObject); //TODO: Research death effects - e.g. instantiate smaller blocks & engage a particle system
            }
        }
    }

    private IEnumerator ShotEffect(Transform target)
    {
        zapSource.gameObject.SetActive(true);
        zapTargeter.target = target;
        yield return spellDuration;
        zapTargeter.target = null;
        zapSource.gameObject.SetActive(false);
    }
    #endregion

#region Confuse Implementation
    private IEnumerator Confuse(Transform target)
    {
        Vector3 oppositeToPlayer = target.position - transform.position;
        Debug.Log("AbilityComponent.cs: Add enemy TYPE to confuse line 103");
        //Enemy enemy = target.GetComponent<Enemy>();
        //enemy.Move(oppositeToPlayer);
        yield return new WaitForSeconds(confuseDuration);
        //enemy.ResetDestination();
    }
#endregion

#region AoE Implementation
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