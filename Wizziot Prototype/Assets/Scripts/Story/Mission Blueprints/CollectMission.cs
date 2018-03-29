using UnityEngine;

[CreateAssetMenu(fileName = "Collect Mission", menuName = "Missions/Collect Mission")]
public class CollectMission : Mission {

    public Item collectItem;

    //Check if item matches the specified "collectItem"
    public override void UpdateMission(Targetable item)
    {
        if (item.GetType() == collectItem.GetType() && (location - item.transform.position).sqrMagnitude < locationRadius)
        {
            CompleteMission();
        }
    }
}
