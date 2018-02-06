using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    private static Inventory _Inventory;
    public static Inventory Instance { get { return _Inventory; } }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChanged;

    public List<Item> items = new List<Item>();
    public int space = 20;

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
        DontDestroyOnLoad(gameObject);
    }

    public bool Add(Item item)
    {
        if(item.targetType == TargetType.Item)
        {
            if(items.Count >= space)
            {
                Debug.Log("Inventory full");
                return false;
            }
            else
            {
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
}
