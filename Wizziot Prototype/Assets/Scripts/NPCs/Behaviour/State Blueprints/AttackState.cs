using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "States/Attack")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class AttackState : State {

    private AbilityComponent abilComponent;

    protected override State EnterState(Enemy owner)
    {
        abilComponent = owner.abilityComponent;
        //abilComponent.globalCooldown = owner.stats.

        return base.EnterState(owner);
    }

    public override void Execute()
    {
        int randomIndex = Random.Range(0, abilComponent.unlockedAbilities.Count);
        Abilities ability = abilComponent.unlockedAbilities[randomIndex];

        abilComponent.SelectAbility(ability);

        target = SelectTarget();
        if (target != null)
        {
            if (HostileToCurrentTarget())
            {
                if ((target.position - owner.Position).sqrMagnitude < owner.stats.sqrMaxTargetDistance)
                {
                    if(owner.stats.Damage(ability)) abilComponent.UseSelected(target);  //TODO: Make UseSelected a bool - successful hits can inspire enemy (influence emotionchip)
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