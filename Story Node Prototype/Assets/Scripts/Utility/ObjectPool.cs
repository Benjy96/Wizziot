using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for storing gameobjects. Prevents unneeded instantiation. Good for things like bullets, etc.
/// </summary>
/// <typeparam name="T"></typeparam>
class ObjectPool<T> where T : Object    //Constraint on type : must be or derive from a UnityEngine object
{
    public T prefab;
    private List<T> inactiveObjects = new List<T>();

    public T GetObject()
    {
        T objectToReturn;

        if (inactiveObjects.Count > 0)
        {
            objectToReturn = inactiveObjects[0];
            inactiveObjects.RemoveAt(0);
        }
        else
        {
            objectToReturn = Object.Instantiate(prefab);
        }

        return objectToReturn;
    }

    public void ReturnObject(T objectToPool)
    {
        inactiveObjects.Add(objectToPool);
    }
}