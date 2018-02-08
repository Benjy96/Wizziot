using UnityEngine;

//TODO: Research generics to make a mission class/classes

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission")]
public class Mission : ScriptableObject {

    bool completed = false;

    new public string name = "";
    public Vector3 waypoint;
}
