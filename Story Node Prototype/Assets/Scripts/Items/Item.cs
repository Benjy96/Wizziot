using UnityEngine;

public class Item : Targetable {

    protected new TargetType targetType = TargetType.Item;

    public ItemAttributes itemAttributes;
    public float interactionRadius = 5f;

    private void Start()
    {
        interactionRadius *= interactionRadius;
    }

    public void PickUp(Transform player)
    {
        if((player.transform.position - transform.position).sqrMagnitude < interactionRadius)
        {
            Debug.Log("Adding " + itemAttributes.name + " to inventory");
            Inventory.Instance.Add(this);
            Destroy(gameObject);
        }
    }
}
