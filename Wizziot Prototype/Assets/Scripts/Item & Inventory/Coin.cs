using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameMetaInfo._TAG_SHOOTABLE_BY_NPC))
        {
            Inventory.Instance.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
