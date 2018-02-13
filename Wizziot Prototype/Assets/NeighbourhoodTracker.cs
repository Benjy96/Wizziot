using System.Collections.Generic;
using UnityEngine;

//This class keeps track of nearby Enemies (agents)
class NeighbourhoodTracker : MonoBehaviour
{
    public EnemySpawnPoint spawn;

    private List<Enemy> neighbours;
    private SphereCollider sphereCol;

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

    public Vector3 AvgTooClosePos
    {
        get
        {
            Vector3 avg = Vector3.zero;
            Vector3 delta;
            int nearCount = 0;
            for (int i = 0; i < neighbours.Count; i++)
            {
                delta = neighbours[i].Position - transform.position;
                if (delta.magnitude <= spawn.collisionDistance)
                {
                    avg += neighbours[i].Position;
                    nearCount++;
                }
            }
            if (nearCount == 0) return avg;
            avg /= nearCount;
            return avg;
        }
    }

    private void Start()
    {
        neighbours = new List<Enemy>();
        sphereCol = GetComponent<SphereCollider>();
        sphereCol.radius = spawn.neighbourDistance / 2;
    }

    private void FixedUpdate()
    {
        if (sphereCol.radius != spawn.neighbourDistance / 2) sphereCol.radius = spawn.neighbourDistance / 2;
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
            }
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
    }
}