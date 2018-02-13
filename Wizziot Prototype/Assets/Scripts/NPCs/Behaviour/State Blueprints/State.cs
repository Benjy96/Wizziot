﻿using System;
using UnityEngine;

public class State : ScriptableObject {

    /// <summary>
    /// Used to create a Scriptable Object instance
    /// </summary>
    /// <param name="owner">The agent attempting to create/use the specified state</param>
    /// <returns></returns>
    public State CreateState(Enemy owner)
    {
        State newState = (State)CreateInstance(GetType().Name);
        return newState.EnterState(owner);
    }

    /// <summary>
    /// Use as a constructor to init local variables
    /// </summary>
    /// <param name="owner">The agent using this state</param>
    /// <returns></returns>
    protected virtual State EnterState(Enemy owner)
    {
        Debug.Log("EnterState(): Implement a Constructor entry method for the state");
        return null;
    }

    /// <summary>
    /// Agent accesses this method, which then uses a private implementation
    /// </summary>
    public virtual void Execute()
    {
        Debug.Log("State.Execute(): Override this method");
    }

    /// <summary>
    /// Use to gracefully exit a state - e.g. stop Coroutines, etc.
    /// </summary>
    public virtual void ExitState()
    {
        Debug.Log("ExitState() acts as a kind of Destructor for the state");
    }
}