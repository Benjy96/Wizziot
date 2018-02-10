using UnityEngine;

public class SphereNPC : InteractableNPC
{
    public float pushOffForce = 5f;

    //Called by parent class Start() method
    protected override void RegisterExternalFunctions()
    {
        storyManager.BindExternalFunction("SpherePushOff", SpherePushOff);
    }

    //Called from Ink script once bound as an external function
    private void SpherePushOff()
    {
        Vector3 away = (player.transform.position - transform.position).normalized;
        away *= pushOffForce;

        player.GetComponent<Rigidbody>().AddForce(away, ForceMode.Impulse);
    }
}
