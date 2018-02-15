using UnityEngine;

public class PatrolState : State {

    public override void Execute()
    {
        Patrol();
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
