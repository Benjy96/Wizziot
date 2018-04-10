using UnityEngine;

[System.Serializable]
public class Item : Targetable {

    new public string name = "New item";
    public Sprite icon = null;
    public Equipment equipment;

    private float interactionRadius = 5f;

    private void Awake()
    {
        equipment.prefabName = gameObject.name.Split('(')[0];
        targetType = TargetType.Item;
    }

    private void Start()
    {
        //Convert for use with sqr ranges
        interactionRadius *= interactionRadius;
    }

    /// <summary>
    /// AKA PickUp();
    /// Adds an item object to the player's inventory.
    /// Stores the ScriptableObject in inventory/applies to stats.
    /// Parents the GameObject to the player and disables it.
    /// </summary>
    /// <param name="player"></param>
    public void AddToInventory()
    {
        if ((PlayerManager.Instance.player.transform.position - transform.position).sqrMagnitude < interactionRadius)
        {
            if (Inventory.Instance.Add(this))
            {
                MissionManager.Instance.RegisterItemFound(this);
            }
        }
    }

    public void AddToInventoryFromSaveFile()
    {
        Inventory.Instance.Add(this);
        MissionManager.Instance.RegisterItemFound(this);
        gameObject.SetActive(false);
        gameObject.transform.SetParent(PlayerManager.Instance.player.transform);
        gameObject.transform.position = PlayerManager.Instance.player.transform.position;
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
