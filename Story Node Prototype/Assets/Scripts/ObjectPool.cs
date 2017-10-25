using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Constraint on type : must be or derive from a UnityEngine object
class ObjectPool<T> where T : Object
{
    public T prefab;
    private Stack<T> inactiveObjects = new Stack<T>();

    public T GetObject()
    {
        T objectToReturn;

        if (inactiveObjects.Count > 0)
        {
            objectToReturn = inactiveObjects.Pop();
        }
        else
        {
            objectToReturn = Object.Instantiate(prefab);
        }

        return objectToReturn;
    }

    public void ReturnObject(T objectToPool)
    {
        inactiveObjects.Push(objectToPool);
    }
}