using UnityEngine;

public class Coin : MonoBehaviour {

    public int coinValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(GameMetaInfo._TAG_SHOOTABLE_BY_NPC))
        {
            Inventory.Instance.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
