﻿using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Collect Mission", menuName = "Missions/Collect Mission")]
public class CollectMission : Mission {

    [JsonIgnore] public GameObject collectItem;

    //Check if item matches the specified "collectItem"
    public override void UpdateMission(Targetable item)
    {
        string itemName = item.name.Split('(')[0];  //Disregard clones
        if (itemName.Equals(collectItem) && (location - item.transform.position).sqrMagnitude < waypointRadius * waypointRadius)
        {
            completed = true;
            return;
        }
    }
}
