using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Kill Mission", menuName = "Missions/Kill Mission")]
public class KillMission : Mission {

    public List<Targetable> killTargets;    //TODO: Replace with type enum
    public int killsRequired;
}
