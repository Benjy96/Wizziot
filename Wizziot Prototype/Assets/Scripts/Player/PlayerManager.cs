using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private static PlayerManager _PlayerManager;
    public static PlayerManager Instance { get { return _PlayerManager; } }

    public GameObject player;   //reference for runtime
    public PlayerController playerControls;
    public Item[] equipped;

    public Action onTargetDestroyed;    //for targeting, xp, etc...

    private EntityStats playerStats;

    private void Awake()
    {
        //Singleton setup
        if (_PlayerManager == null)
        {
            _PlayerManager = this;
        }
        else if (_PlayerManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        playerStats = player.GetComponent<EntityStats>();
        playerControls = player.GetComponent<PlayerController>();
        equipped = new Item[Enum.GetValues(typeof(EquipmentSlot)).Length];
    }

    /// <summary>
    /// Equip an item and apply its modifiers to the player's stats
    /// </summary>
    /// <param name="stats">The modifiers you wish to apply</param>
    public void EquipItem(Item item)
    {
        Debug.Log("Equipping " + item.equipment.name);

        Equipment newEquipment = item.equipment;
        int equipmentSlot = (int) newEquipment.slot;

        Item oldEquipmentItem; //temp

        //If equipment already in slot, switch them
        if(equipped[equipmentSlot] != null)
        {
            oldEquipmentItem = equipped[equipmentSlot];
            Inventory.Instance.Add(oldEquipmentItem);   //what is the item/
        }
        equipped[equipmentSlot] = item;
        Inventory.Instance.Remove(item);

        //Get key
        foreach (Stat stat in newEquipment.modifiers)
        {
            //Set new stat value (apply modifier)
            playerStats.statModifiers[stat.StatType] = stat;
        }

        //Make item visible and enabled, disable collider
        item.gameObject.SetActive(true);
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.enabled = false;

        //Add to player model
        switch (newEquipment.slot)
        {
            case EquipmentSlot.Head:
                EquipHead(item);
                break;

            case EquipmentSlot.Body:
                EquipBody(item);
                break;

            case EquipmentSlot.Pendant:
                EquipPendant(item);
                break;

            case EquipmentSlot.Weapon:
                EquipWeapon(item);
                break;
        }

        playerStats.ApplyStatModifiers();
    }

    //TODO: make more complicated, needlessly
    void EquipHead(Item item)
    {
        item.transform.parent = player.transform;

        item.transform.localPosition = new Vector3(0f, player.transform.localScale.y / 2, 0f);
        item.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        //Disable everything except component visibility
        //MonoBehaviour[] itemComponents = item.GetComponents<MonoBehaviour>();
        //foreach (MonoBehaviour component in itemComponents)
        //{
        //    if (component.GetType() == typeof(Transform)) continue;
        //    component.enabled = false;
        //}
    }

    void EquipBody(Item item)
    {
        item.gameObject.SetActive(true);
        item.transform.parent = player.transform;
        item.transform.position = new Vector3(player.transform.position.x,
            player.transform.position.y,
            player.transform.position.z);
    }

    void EquipPendant(Item item)
    {
        item.gameObject.SetActive(true);
        item.transform.parent = player.transform;
        item.transform.position = new Vector3(player.transform.position.x,
            (player.transform.position.y + player.transform.localScale.y),
            player.transform.position.z);
    }

    void EquipWeapon(Item item)
    {
        item.gameObject.SetActive(true);
        item.transform.parent = player.transform;
        item.transform.position = new Vector3(player.transform.position.x,
            (player.transform.position.y + player.transform.localScale.y),
            player.transform.position.z);
    }
}
