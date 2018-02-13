using UnityEngine;

public class State : ScriptableObject {

    public virtual State EnterState(Enemy owner)
    {
        return null;
    }

    public virtual void Execute()
    {
        Debug.Log("State.Execute(): Override this method");
    }
}
