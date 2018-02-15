using UnityEngine;

public class PatrolState : State {

    protected override State EnterState(Enemy owner)
    {
        return base.EnterState(owner);
    }

    public override void Execute()
    {
        Patrol();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Patrol()
    {
        if (owner.DestinationReached())
        {
            int randomIndex = Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count);
            owner.MoveTo(owner.Spawn.spawnAreaWaypoints[randomIndex]);
        }
    }
}
