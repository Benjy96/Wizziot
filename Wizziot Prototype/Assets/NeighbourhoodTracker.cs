using System;
using System.Collections.Generic;
using UnityEngine;

//This class keeps track of nearby Enemies (agents)
public class NeighbourhoodTracker : MonoBehaviour
{
    public List<Enemy> neighbours;

    public Dictionary<string, GameObject> secondaryNeighbours;

    private EnemySpawnPoint spawnPoint;
    private SphereCollider sphereCol;

    public float TrackingRadius { get { return sphereCol.radius; } set { sphereCol.radius = value; } }

    public Vector3 AvgPos
    {
        get
        {
            Vector3 avg = Vector3.zero;
            if (neighbours.Count == 0) return avg;

            for (int i = 0; i < neighbours.Count; i++)
            {
                avg += neighbours[i].Position;
            }
            avg /= neighbours.Count;
            return avg;
        }
    }

    public Vector3 AvgVel
    {
        get
        {
            Vector3 avg = Vector3.zero;
            if (neighbours.Count == 0) return avg;

            for (int i = 0; i < neighbours.Count; i++)
            {
                avg += neighbours[i].Velocity;
            }
            avg /= neighbours.Count;
            return avg;
        }
    }

    private void Awake()
    {
        neighbours = new List<Enemy>();
        secondaryNeighbours = new Dictionary<string, GameObject>();

        sphereCol = GetComponent<SphereCollider>();
    }

    //Add enemy to neighbourhood on trigger collision
    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e != null)
        {
            if (neighbours.IndexOf(e) == -1)
            {
                neighbours.Add(e);
                return;
            }
        }

        if (secondaryNeighbours.ContainsKey(other.name))
        {
            secondaryNeighbours[other.name] = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e != null)
        {
            if (neighbours.IndexOf(e) != -1)
            {
                neighbours.Remove(e);
            }
        }

        if (secondaryNeighbours.ContainsKey(other.name))
        {
            if (secondaryNeighbours[other.name] != null)
            {
                secondaryNeighbours[other.name] = null;
            }
        }
    }

    //Using dictionary to register object references that are able to be tracked - O(1) access time
    public void RegisterInterest(GameObject gameObject)
    {
        secondaryNeighbours.Add(gameObject.name, null);
    }

    public GameObject RetrieveTrackedObject(GameObject gameObject)
    {
        if (secondaryNeighbours.ContainsKey(gameObject.name))
        {
            if(secondaryNeighbours[gameObject.name] != null)
            {
                return secondaryNeighbours[gameObject.name];
            }
        }

        return null;
    }
}