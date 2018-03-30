using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Kill Mission", menuName = "Missions/Kill Mission")]
public class KillMission : Mission {

    public List<GameObject> killTypes;
    public List<int> killsRequired;

    //Check if enemy matches any of the specified "killTypes"
    public override void UpdateMission(Targetable enemy)
    {
        int killTypeIndex = 0;
        string enemyName = enemy.name.Split('(')[0];    //disregard clone part of name
        
        foreach (GameObject x in killTypes)
        {
            //If enemy matches type in kill types and the distance is within range of the waypoint, update the quest
            if(x.name.Equals(enemyName) && (location - enemy.transform.position).sqrMagnitude < (waypointRadius * waypointRadius))
            {
                Debug.Log("Quest conditions met, updating progress");
                killsRequired[killTypeIndex]--;
                if (killsRequired[killTypeIndex] <= 0)
                {
                    completed = true;
                    return;
                }
                killTypeIndex++;
            }
        }
    }
}