using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    private static Inventory _Inventory;
    public static Inventory Instance { get { return _Inventory; } }

    public Action onItemChanged;
    public Action onCoinPickup;

    public InventoryUIManager inventoryUI;

    public int space = 20;

    public List<Item> items = new List<Item>();
    public int coins;

    private void Awake()
    {
        //Singleton setup
        if (_Inventory == null)
        {
            _Inventory = this;
        }
        else if (_Inventory != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    public bool Add(Item item)
    {
        Debug.Log("Adding item: " + item.name + ", item target type: " + item.targetType);
        if(item.targetType == TargetType.Item)
        {
            if(items.Count >= space)
            {
                Debug.Log("Inventory full");
                return false;
            }
            else
            {
                Debug.Log("Updating inventory");
                item.gameObject.SetActive(false);
                item.gameObject.transform.SetParent(PlayerManager.Instance.player.transform);
                item.gameObject.transform.position = PlayerManager.Instance.player.transform.position;

                items.Add(item);
                if(onItemChanged != null) onItemChanged.Invoke();
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public bool Remove(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            if (onItemChanged != null) onItemChanged.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Drop(Item item)
    {
        Remove(item);
        gameObject.transform.SetParent(null);
        gameObject.SetActive(true);
    }

    //Add coins if the player will not go over the coin limit determined in GameMetaInfo
    public void AddCoins(int count)
    {
        if(count > 0)
        {
            if ((coins + count) < GameMetaInfo._MAX_COINS)
            {
                coins += count;
                if (onCoinPickup != null) onCoinPickup.Invoke();
            }
        }
        else
        {
            RemoveCoins(count);
        }
        
    }

    public void RemoveCoins(int count)
    {
        if(count < 0 && (coins - count) >= 0)
        {
            coins -= count;
            if (onCoinPickup != null) onCoinPickup.Invoke();
        }
        else
        {
            AddCoins(count);
        }
    }
}
