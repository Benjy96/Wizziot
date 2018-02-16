using UnityEngine;

public class ShopKeeper : InteractableNPC
{
    protected override void RegisterExternalFunctions()
    {
        storyManager.BindExternalFunction("GrantMission", GrantMission);
    }

    private void GrantMission()
    {
        MissionManager.Instance.GrantMission(mission);
    }
}
