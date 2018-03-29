using UnityEngine;

public class Item : Targetable {

    new public string name = "New item";
    public Sprite icon = null;
    public Equipment equipment;

    private float interactionRadius = 5f;

    private void Awake()
    {
        targetType = TargetType.Item;
    }

    private void Start()
    {
        //Convert for use with sqr ranges
        interactionRadius *= interactionRadius;
    }

    public void PickUp(Transform player)
    {
        if ((player.transform.position - transform.position).sqrMagnitude < interactionRadius)
        {
            Inventory.Instance.Add(this);
            MissionManager.Instance.RegisterItemFound(this);
            gameObject.SetActive(false);
            gameObject.transform.SetParent(player);
            //Destroy(gameObject); //use this if going to use ScriptableObjects <- destroy the ITEM, store the asset
        }
    }

    public void Use()
    {
        if(equipment != null)
        {
            PlayerManager.Instance.EquipItem(this);
        }
        else
        {
            Inventory.Instance.Drop(this);
        }
    }
}
