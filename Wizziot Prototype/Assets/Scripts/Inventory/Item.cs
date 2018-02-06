using UnityEngine;

public class Item : Targetable {

    private Transform player;

    public float interactionRadius = 5f;

    new public string name = "New item";
    public Sprite icon = null;

    private void Start()
    {
        interactionRadius *= interactionRadius;
    }

    public void PickUp(Transform player)
    {
        this.player = player;

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
