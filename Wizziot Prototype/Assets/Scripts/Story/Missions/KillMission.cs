using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Kill Mission", menuName = "Missions/Kill Mission")]
public class KillMission : Mission {

    public List<Enemy> killTypes;
    public int killsRequired;

    public bool RegisterKill(Enemy enemy)
    {
        foreach (Enemy x in killTypes)
        {
            if(x.GetType() == enemy.GetType())
            {
                killsRequired -= 1;
                if (killsRequired == 0) CompleteMission();
                return true;
            }
        }
        return false;
    }
}
