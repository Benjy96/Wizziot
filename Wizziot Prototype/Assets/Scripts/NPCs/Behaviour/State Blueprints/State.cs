﻿using UnityEngine;

public class State : ScriptableObject {

    public GameObject interestedIn;
    public GameObject secondaryInterest;
    public bool hostileToInterests;

    [HideInInspector] public GameObject influencer;   //use this when influencing - could be good for WHO to hide from or attack

    protected Enemy owner;
    protected EnemySpawnPoint spawn;
    protected NeighbourhoodTracker neighbourhoodTracker;

    protected Transform target;
    
    /// <summary>
    /// Used to create a Scriptable Object instance
    /// </summary>
    /// <param name="owner">The agent attempting to create/use the specified state</param>
    /// <returns></returns>
    public State CreateState(Enemy owner, GameObject lastInfluence)
    {
        State newState = (State)Instantiate(Resources.Load("State Objects/" + name));
        newState.EnterState(owner, lastInfluence);
        return newState;
    }

    /// <summary>
    /// Use as a constructor to init local variables
    /// </summary>
    /// <param name="owner">The agent using this state</param>
    /// <returns></returns>
    protected virtual void EnterState(Enemy owner, GameObject lastInfluence)
    {
        this.owner = owner;
        influencer = lastInfluence;
        spawn = owner.Spawn;
        neighbourhoodTracker = owner.neighbourhoodTracker;

        neighbourhoodTracker.TrackOtherEnemies();
        neighbourhoodTracker.RegisterInterest(interestedIn);
        neighbourhoodTracker.RegisterInterest(secondaryInterest);
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
        neighbourhoodTracker.RemoveInterest(interestedIn);
        neighbourhoodTracker.RemoveInterest(secondaryInterest);
    }

    protected virtual Transform SelectTarget()
    {
        if (influencer != null && (influencer.transform.position - owner.Position).magnitude < owner.SightRange)
        {
            return influencer.transform;
        }
        else
        {
            GameObject targetGO = null;
            Transform target = null;

            targetGO = neighbourhoodTracker.RetrieveTrackedObject(interestedIn);

            if (targetGO != null) target = targetGO.transform;

            if (target == null && owner.target != null) target = owner.target;

            return target;
        }
    }
}