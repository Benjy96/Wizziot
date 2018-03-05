using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "States/Attack")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class AttackState : State {

    private AbilityComponent abilComponent;
    private PlayerStats playerStats;

    protected override void EnterState(Enemy owner, GameObject lastInfluence)
    {
        base.EnterState(owner, lastInfluence);
    }

    public override void Execute()
    {
        int randomIndex = Random.Range(0, abilComponent.unlockedAbilities.Count);
        Abilities ability = abilComponent.unlockedAbilities[randomIndex];

        abilComponent.SelectAbility(ability);

        Transform newTarget = SelectTarget();
        if(newTarget != target)
        {
            target = newTarget;
            playerStats = target.GetComponent<PlayerStats>();
        }

        if (target != null)
        {
            if (HostileToCurrentTarget())
            {
                if ((target.position - owner.Position).sqrMagnitude < owner.stats.sqrMaxTargetDistance)
                {
                    if(abilComponent.UseSelected(target)) playerStats.Damage(1f);
                }
            }
            owner.FaceTarget(target.position);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private bool HostileToCurrentTarget()
    {
        if (target == null) return false;

        if (target.name == interestedIn.name && !hostileToInterests) return false;
        
        if (target.tag.Equals(GameMetaInfo._TAG_SHOOTABLE_BY_NPC))
        {
            return true;
        }

        return false;
    }
}