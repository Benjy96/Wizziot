using System;
using System.Collections.Generic;
using UnityEngine;

public class State : ScriptableObject {

    protected Enemy owner;

    /// <summary>
    /// Used to create a Scriptable Object instance
    /// </summary>
    /// <param name="owner">The agent attempting to create/use the specified state</param>
    /// <returns></returns>
    public State CreateState(Enemy owner)
    {
        State newState = (State)Instantiate(Resources.Load("State Objects/" + GetType().Name));
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
        return this;
    }

    /// <summary>
    /// Agent accesses this method, which then uses a private implementation
    /// </summary>
    public virtual void Execute()
    {
        Debug.Log("State.Execute(): Add state behaviour here. Use fallbackBehaviour.Execute() if your state's conditions fail.");
    }

    /// <summary>
    /// Use to gracefully exit a state - e.g. stop Coroutines, etc...
    /// </summary>
    /// <returns>The next state to enter. Returning null (by default) means the next State will be handled by the EmotionChip</returns>
    public virtual void ExitState()
    {
        Debug.Log("Return a new State if you wish to switch to a new state directly from within another state");
    }
}
