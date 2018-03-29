using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Kill Mission", menuName = "Missions/Kill Mission")]
public class KillMission : Mission {

    public List<GameObject> killTypes;
    public int killsRequired;

    public override bool UpdateMission(Targetable enemy)
    {
        foreach (GameObject x in killTypes)
        {
            if(x.GetType() == enemy.GetType() && (location - x.transform.position).sqrMagnitude < locationRadius)
            {
                killsRequired -= 1;
                if (killsRequired == 0) CompleteMission();
                return true;
            }
        }
        return false;
    }
}