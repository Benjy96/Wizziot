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
        item.gameObject.SetActive(true);
        item.transform.parent = player.transform;
        item.transform.position = new Vector3(player.transform.position.x,
            (player.transform.position.y + player.transform.localScale.y),
            player.transform.position.z);
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
