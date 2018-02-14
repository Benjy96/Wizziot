using System;
using UnityEngine;

public class State : ScriptableObject {

    protected State defaultState;   //What to do if the selected state cannot be fulfilled

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
        Debug.Log("State.Execute(): Override this method");
    }

    /// <summary>
    /// Use to gracefully exit a state - e.g. stop Coroutines, etc. Returns a state to enter next if needed.
    /// </summary>
    /// <returns>The next state to enter (only return this if the current state has failed and needs to "default"</returns>
    public virtual State ExitState()
    {
        Debug.Log("ExitState() acts as a kind of Destructor for the state");
        return null;
    }
}
