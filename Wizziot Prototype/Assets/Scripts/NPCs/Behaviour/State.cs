using UnityEngine;

public class State : ScriptableObject {

    //Create a Scriptable Object instance
    public virtual State EnterState(Enemy owner)
    {
        Debug.Log("Create an instance of the ScriptableObject asset in here");
        return null;
    }

    //Initialize object local variables
    protected virtual State SetupState(Enemy owner)
    {
        Debug.Log("EnterState(): Implement a Constructor entry method for the state");
        return null;
    }

    public virtual void Execute()
    {
        Debug.Log("State.Execute(): Override this method");
    }

    public virtual void ExitState()
    {
        Debug.Log("ExitState() acts as a kind of Destructor for the state");
    }
}
