using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "States/Attack")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class AttackState : State {

    private AbilityComponent abilComponent;

    protected override State EnterState(Enemy owner)
    {
        target = owner.target;
        abilComponent = owner.abilityComponent;

        return base.EnterState(owner);
    }

    public override void Execute()
    {
        int randomIndex = Random.Range(0, abilComponent.unlockedAbilities.Count);
        Abilities ability = abilComponent.unlockedAbilities[randomIndex];

        abilComponent.SelectAbility(ability);

        if ((target.position - owner.Position).sqrMagnitude < owner.stats.sqrMaxTargetDistance)
        {
            abilComponent.UseSelected(target);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}