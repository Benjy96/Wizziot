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
            Debug.Log(name + " is targeting " + target.name);
            if (HostileToCurrentTarget())
            {
                Debug.Log(name + " is hostile to " + target.name);
                if ((target.position - owner.Position).sqrMagnitude < owner.stats.sqrMaxTargetDistance)
                {
                    //If not a defense ability, use on the target
                    if(!GameMetaInfo._Is_Defense_Ability(abilComponent.SelectedAbility)) abilComponent.UseSelected(target);
                }
            }
            else
            {
                abilComponent.SelectAbility(Abilities.Heal);
                abilComponent.UseSelected(target);
            }
            owner.FaceTarget(target.position);
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