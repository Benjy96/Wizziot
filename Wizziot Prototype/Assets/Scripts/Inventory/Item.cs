using UnityEngine;

//TODO: ScriptableObject?
//TODO: Equippable item / State modifiers
//TODO: Collider items e.g. coins
public class Item : Targetable {

    public float interactionRadius = 5f;

    new public string name = "New item";
    public Sprite icon = null;

    private void Start()
    {
        interactionRadius *= interactionRadius;
    }

    public virtual void PickUp(Transform player)
    {
        if((player.transform.position - transform.position).sqrMagnitude < interactionRadius)
        {
            Inventory.Instance.Add(this);
            gameObject.SetActive(false);
            gameObject.transform.SetParent(player);
            //Destroy(gameObject); //use this if going to use ScriptableObjects <- destroy the ITEM, store the asset
        }
    }

    public virtual void Use()
    {
        Inventory.Instance.Remove(this);
        gameObject.transform.SetParent(null);
        gameObject.SetActive(true);
    }
}
