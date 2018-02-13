using UnityEngine;

public class State : ScriptableObject {

    public virtual void Execute(Enemy owner)
    {
        Debug.Log("State.Execute(): Override this method");
    }
}
