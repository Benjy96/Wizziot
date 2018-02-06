using UnityEngine;

public class Item : Targetable {

    public float interactionRadius = 5f;
    public ItemAttributes itemAttributes;

    private void Start()
    {
        interactionRadius *= interactionRadius;
    }

    public void PickUp(Transform player)
    {
        if((player.transform.position - transform.position).sqrMagnitude < interactionRadius)
        {
            Inventory.Instance.Add(this);
            Destroy(gameObject);
        }
    }
}
