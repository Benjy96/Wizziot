using UnityEngine;

[CreateAssetMenu(fileName = "SuicideState", menuName = "States/Suicide")]
public class SuicideState : State {

    public string lastWords;

    private bool suicidal;
    private bool reachedFinalDestination;

    public override void Execute()
    {
        Debug.Log(lastWords);
        if (!suicidal)
        {
            suicidal = true;
            int randomIndex = Random.Range(0, owner.Spawn.spawnAreaWaypoints.Count);
            owner.MoveTo(owner.Spawn.spawnAreaWaypoints[randomIndex]);
        }

        if (owner.DestinationReached())
        {
            reachedFinalDestination = true;
        }

        if (reachedFinalDestination)
        {
            ExitState();
            Destroy(owner.gameObject);
        }
    }

    public override void ExitState()
    {
        foreach (Enemy e in owner.neighbourhoodTracker.neighbours)
        {
            e.neighbourhoodTracker.neighbours.Remove(owner);
        }
    }
}
