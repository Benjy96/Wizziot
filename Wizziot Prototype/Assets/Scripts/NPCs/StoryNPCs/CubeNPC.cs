using UnityEngine;

public class CubeNPC : InteractableNPC
{
    public float pushOffForce = 5f;

    protected override void RegisterExternalFunctions()
    {
        storyManager.BindExternalFunction("CubePushOff", CubePushOff);
        storyManager.BindExternalFunction("CubeRevenge", CubeRevenge);
    }

    private void CubeRevenge()
    {
        MissionManager.Instance.GrantMission(mission);
    }

    private void CubePushOff()
    {
        Vector3 away = (player.transform.position - transform.position).normalized;
        away *= pushOffForce;    //set magnitude

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
