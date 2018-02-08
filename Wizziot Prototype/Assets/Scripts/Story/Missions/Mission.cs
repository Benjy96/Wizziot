using System.Collections.Generic;
using UnityEngine;

//TODO: Research generics to make a mission class/classes

[CreateAssetMenu(fileName = "Standard Mission", menuName = "Missions/Standard Mission")]
public class Mission : ScriptableObject {

    bool completed = false;
    new public string name = "";
    public Stack<Mission> missionStages;
}
