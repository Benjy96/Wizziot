using UnityEngine;

[CreateAssetMenu(fileName = "AttackState", menuName = "States/Attack")]
[RequireComponent(typeof(NeighbourhoodTracker))]
public class AttackState : State {

    /// <summary>
    /// Select a random ability and attempt to use on the selected target. Follows target.
    /// Otherwise, move about in a circle and attepmt to calm down.
    /// </summary>
    public override void Execute()
    {
        int randomIndex = Random.Range(0, abilComponent.unlockedAbilities.Count);
        Abilities ability = abilComponent.unlockedAbilities[randomIndex];

        abilComponent.SelectAbility(ability);

        Transform newTarget = SelectTarget();
        if(newTarget != target)
        {
            target = newTarget;
        }

        if (target != null)
        {
            //if target in range
            if((target.position - owner.Position).sqrMagnitude < owner.stats.sqrMaxTargetDistance)
            {
                //if hostile target
                if (HostileToCurrentTarget())
                {
                    //if offense ability, attack
                    if (!GameMetaInfo._Is_Defense_Ability(abilComponent.SelectedAbility))
                    {
                        abilComponent.UseSelected(target);
                    }
                    //if defense ability, use on self if hostile target
                    else
                    {
                        abilComponent.UseSelected(owner.transform);
                    }
                }
                //if target not hostile, but ability is a defense ability, use it on them
                else if(GameMetaInfo._Is_Defense_Ability(abilComponent.SelectedAbility))
                {
                    abilComponent.UseSelected(target);
                }
            }
            owner.MoveTo(target.position);
        }
        else
        {
            owner.MoveTo(new Vector3(owner.Position.x + Mathf.Cos(Time.time) * 2f, 0f, owner.Position.z + Mathf.Sin(Time.time) * 2f));
            owner.Influence(Emotion.Calm, .2f * Time.fixedDeltaTime);
        }
    }

    private bool HostileToCurrentTarget()
    {
        if (target == null) return false;

        if (interestedIn != null && (target.name == interestedIn.name && hostileToInterests))
        {
            return true;
        }

        if (secondaryInterest != null && (target.name == secondaryInterest.name && hostileToInterests))
        {
            return true;
        }
        
        if (target.tag.Equals(GameMetaInfo._TAG_SHOOTABLE_BY_NPC))
        {
            return true;
        }

        return false;
    }
}