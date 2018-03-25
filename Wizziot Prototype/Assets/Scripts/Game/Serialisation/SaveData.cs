using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data class that holds various key-value pairs (as part of a nested generic class) for serialisations.
/// A SaveData object has dictionaries that can be saved to and accessed using overloaded generic methods. (Load() and Save())
/// Load() will retrieve a value using a provided key and set your output via reference.
/// Save() will assign to a key value pair of corresponding type.
/// </summary>
[Serializable]
public class SaveData
{
    //The reason we use generics is so that the class is truly acting as a BLUEPRINT. i.e. we create structures that can vary in type, and their methods work for each.
    [Serializable]
    public class SaveDictionary<T>
    {
        public int Length { get { return keys.Count; } }

        public List<string> keys = new List<string>();
        public List<T> values = new List<T>();

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void TrySetValue(string key, T value)
        {
            int index = keys.FindIndex(x => x == key);

            if (index > -1)
            {
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public bool TryGetValue(string key, ref T value)
        {
            int index = keys.FindIndex(x => x == key);  //return x where x is equal to the key

            if (index > -1)
            {
                value = values[index];
                return true;
            }
            else
            {
                return false;
            }
        }
    }//SaveDictionary

    //Data information
    public int savedItems = 0;

    //The data holders
    public SaveDictionary<bool> boolData = new SaveDictionary<bool>();
    public SaveDictionary<string> stringData = new SaveDictionary<string>();
    public SaveDictionary<int> intData = new SaveDictionary<int>();
    public SaveDictionary<Vector3> v3Data = new SaveDictionary<Vector3>();
    public SaveDictionary<Quaternion> quaternionData = new SaveDictionary<Quaternion>();
    //Custom game types
    public SaveDictionary<List<Item>> inventoryData = new SaveDictionary<List<Item>>();
    public SaveDictionary<List<Mission>> missionData = new SaveDictionary<List<Mission>>();
    public SaveDictionary<Item[]> itemData = new SaveDictionary<Item[]>();

    public void Reset()
    {
        boolData.Clear();
        stringData.Clear();
        intData.Clear();
        v3Data.Clear();
        quaternionData.Clear();

        inventoryData.Clear();
        itemData.Clear();
        missionData.Clear();
    }

    private void Save<T>(SaveDictionary<T> lists, string key, T value)
    {
        savedItems++;
        lists.TrySetValue(key, value);
    }

    private bool Load<T>(SaveDictionary<T> lists, string key, ref T value)
    {
        return lists.TryGetValue(key, ref value);
    }

    //Override methods for GENERIC save function
    public void Save(string key, bool value)
    {
        Save(boolData, key, value);
    }

    public void Save(string key, string value)
    {
        Save(stringData, key, value);
    }

    public void Save(string key, int value)
    {
        Save(intData, key, value);
    }

    public void Save(string key, Vector3 value)
    {
        Save(v3Data, key, value);
    }

    public void Save(string key, Quaternion value)
    {
        Save(quaternionData, key, value);
    }

    public void Save(string key, List<Item> value)
    {
        Save(inventoryData, key, value);
    }

    public void Save(string key, List<Mission> value)
    {
        Save(missionData, key, value);
    }

    public void Save(string key, Item[] value)
    {
        Save(itemData, key, value);
    }

    //Interface Load Overrides
    public bool Load(string key, ref bool value)
    {
        return Load(boolData, key, ref value);
    }

    public bool Load(string key, ref int value)
    {
        return Load(intData, key, ref value);
    }

    public bool Load(string key, ref string value)
    {
        return Load(stringData, key, ref value);
    }

    public bool Load(string key, ref Vector3 value)
    {
        return Load(v3Data, key, ref value);
    }

    public bool Load(string key, ref Quaternion value)
    {
        return Load(quaternionData, key, ref value);
    }

    public bool Load(string key, ref List<Item> value)
    {
        return Load(inventoryData, key, ref value);
    }

    public bool Load(string key, ref List<Mission> value)
    {
        return Load(missionData, key, ref value);
    }

    public bool Load(string key, ref Item[] value)
    {
        return Load(itemData, key, ref value);
    }
}
