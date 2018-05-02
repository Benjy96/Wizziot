using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The reason we use generics is so that the class is truly acting as a BLUEPRINT. i.e. we create structures that can vary in type, and their methods work for each.
/// A data class that holds various key-value pairs (as part of a nested generic class) for serialisations.
/// A SaveData object has dictionaries that can be saved to and accessed using overloaded generic methods. (Load() and Save())
/// Load() will retrieve a value using a provided key and set your output via reference.
/// Save() will assign to a key value pair of corresponding type.
/// </summary>
[Serializable]
public class SaveData
{
    //Key-value pair data structure
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
    }

    //Data information to ensure all game-data has been saved & loaded
    [NonSerialized] public int savedItems = 0;
    [NonSerialized] public int loadedItems = 0;

    //Primitive data holders
    public SaveDictionary<bool> boolData = new SaveDictionary<bool>();
    public SaveDictionary<string> stringData = new SaveDictionary<string>();
    public SaveDictionary<int> intData = new SaveDictionary<int>();
    public SaveDictionary<Vector3> v3Data = new SaveDictionary<Vector3>();
    public SaveDictionary<Quaternion> quaternionData = new SaveDictionary<Quaternion>();
    //Custom game types
    public SaveDictionary<Dictionary<Abilities, KeyCode>> abilityKeyData = new SaveDictionary<Dictionary<Abilities, KeyCode>>();
    public SaveDictionary<List<Mission>> missionData = new SaveDictionary<List<Mission>>();
    public SaveDictionary<List<Equipment>> equipmentData = new SaveDictionary<List<Equipment>>();

    public void Reset()
    {
        boolData.Clear();
        stringData.Clear();
        intData.Clear();
        v3Data.Clear();
        quaternionData.Clear();
        //Custom
        abilityKeyData.Clear();
        missionData.Clear();
        equipmentData.Clear();
    }

    private void Save<T>(SaveDictionary<T> lists, string key, T value)
    {
        savedItems++;
        lists.TrySetValue(key, value);
    }

    private bool Load<T>(SaveDictionary<T> lists, string key, ref T value)
    {
        loadedItems++; 
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

    public void Save(string key, Dictionary<Abilities, KeyCode> value)
    {
        Save(abilityKeyData, key, value);
    }

    public void Save(string key, List<Mission> value)
    {
        Save(missionData, key, value);
    }

    public void Save(string key, List<Equipment> value)
    {
        Save(equipmentData, key, value);
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

    public bool Load(string key, ref Dictionary<Abilities, KeyCode> value)
    {
        return Load(abilityKeyData, key, ref value);
    }

    public bool Load(string key, ref List<Mission> value)
    {
        return Load(missionData, key, ref value);
    }

    public bool Load(string key, ref List<Equipment> value)
    {
        return Load(equipmentData, key, ref value);
    }
} 
