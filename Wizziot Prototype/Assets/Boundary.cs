using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {

    public Vector3 resetPosition = new Vector3(0f, 0f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if(PlayerManager.Instance.playerStats.onDeath != null) PlayerManager.Instance.playerStats.onDeath.Invoke();
            else
            {
                other.transform.position = resetPosition;
                other.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }   
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
