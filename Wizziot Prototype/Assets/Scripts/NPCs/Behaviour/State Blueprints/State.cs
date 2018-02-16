using UnityEngine;

public class State : ScriptableObject {

    public GameObject interestedIn;
    public GameObject secondaryInterest;
    public bool hostileToInterests;

    protected Enemy owner;
    protected EnemySpawnPoint spawn;
    protected NeighbourhoodTracker neighbourhood;

    protected Transform target;
    
    /// <summary>
    /// Used to create a Scriptable Object instance
    /// </summary>
    /// <param name="owner">The agent attempting to create/use the specified state</param>
    /// <returns></returns>
    public State CreateState(Enemy owner)
    {
        State newState = (State)Instantiate(Resources.Load("State Objects/" + name));
        return newState.EnterState(owner);
    }

    /// <summary>
    /// Use as a constructor to init local variables
    /// </summary>
    /// <param name="owner">The agent using this state</param>
    /// <returns></returns>
    protected virtual State EnterState(Enemy owner)
    {
        this.owner = owner;
        spawn = owner.Spawn;
        neighbourhood = owner.neighbourhoodTracker;

        neighbourhood.RegisterInterest(interestedIn);
        neighbourhood.RegisterInterest(secondaryInterest);

        return this;
    }

    /// <summary>
    /// Agent accesses this method, which then uses a private implementation.
    /// </summary>
    public virtual void Execute()
    {
        Debug.Log("State.Execute(): Add State Behaviour Here");
    }

    /// <summary>
    /// Use to gracefully exit a state, remove interest, references, subscriptions - e.g. stop Coroutines, etc...
    /// </summary>
    /// <returns>The next state to enter. Returning null (by default) means the next State will be handled by the EmotionChip</returns>
    public virtual void ExitState()
    {
        neighbourhood.RemoveInterest(interestedIn);
        neighbourhood.RemoveInterest(secondaryInterest);
    }

    protected virtual Transform SelectTarget()
    {
        GameObject targetGO = null;
        Transform target = null;

        targetGO = neighbourhood.RetrieveTrackedObject(interestedIn);

        if (targetGO != null) target = targetGO.transform;

        if (target == null) target = owner.target;

        return target;
    }
}