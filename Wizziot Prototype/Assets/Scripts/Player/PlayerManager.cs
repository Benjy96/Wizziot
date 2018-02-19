using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private static PlayerManager _PlayerManager;
    public static PlayerManager Instance { get { return _PlayerManager; } }

    public GameObject player;

    private PlayerStats playerStats;

    private Item[] equipped;

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

        playerStats = player.GetComponent<PlayerStats>();
        equipped = new Item[Enum.GetValues(typeof(EquipmentSlot)).Length];
    }

    //TODO: Maybe make a separate EquipmentManager?
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

        //TODO: Add to player model
    }
}
