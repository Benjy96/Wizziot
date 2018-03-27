using System.Collections.Generic;
using UnityEngine;

//This class keeps track of nearby Enemies (agents)
public class NeighbourhoodTracker : MonoBehaviour
{
    public List<Enemy> neighbours;
    public List<Transform> obstacles;

    public enum TrackType { Enemies, Obstacles }
    public TrackType toTrack;

    public Dictionary<string, GameObject> secondaryNeighbours;

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

    public void TrackOtherEnemies()
    {
        toTrack = TrackType.Enemies;
        obstacles.Clear();
        ScanForNearby();
    }

    public void TrackObstacles()
    {
        toTrack = TrackType.Obstacles;
        neighbours.Clear();
        ScanForNearby();
    }

    private void Awake()
    {
        neighbours = new List<Enemy>();
        obstacles = new List<Transform>();
        secondaryNeighbours = new Dictionary<string, GameObject>();

        sphereCol = GetComponent<SphereCollider>();

        toTrack = TrackType.Enemies;
    }

    //Add to neighbourhood on trigger collision
    private void OnTriggerEnter(Collider other)
    {
        string[] objName = other.name.Split('(');
        //Track interest
        if (secondaryNeighbours.ContainsKey(objName[0]))
        {
            Debug.Log("tracking " + objName[0]);
            secondaryNeighbours[other.name] = other.gameObject;
        }

        //Track enemies, else environment
        if (toTrack == TrackType.Enemies)
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
        }
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(GameMetaInfo._LAYER_IMMOVABLE_OBJECT))
            {
                Transform t = other.GetComponent<Transform>();
                if (t != null)
                {
                    if (obstacles.IndexOf(t) == -1)
                    {
                        obstacles.Add(t);
                        return;
                    }
                }
            } 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string[] objName = other.name.Split('(');

        //Remove interest
        if (secondaryNeighbours.ContainsKey(objName[0]))
        {
            if (secondaryNeighbours[other.name] != null)
            {
                secondaryNeighbours[other.name] = null;
            }
        }

        //Remove enemies, else environment
        if (toTrack == TrackType.Enemies)
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
        else
        {
            Transform t = other.GetComponent<Transform>();
            if (t != null)
            {
                if (obstacles.IndexOf(t) != -1)
                {
                    obstacles.Remove(t);
                }
            }
        }
    }

    //Using dictionary to register object references that are able to be tracked - O(1) access time
    public void RegisterInterest(GameObject gameObject)
    {
        if (gameObject == null) return;

        Debug.Log("registering " + gameObject.name);

        if (!secondaryNeighbours.ContainsKey(gameObject.name))
        {
            secondaryNeighbours.Add(gameObject.name, null);
        }
    }

    //Call this method when a State is being exited
    public void RemoveInterest(GameObject gameObject)
    {
        if (gameObject == null) return;

        if (secondaryNeighbours.ContainsKey(gameObject.name))
        {
            secondaryNeighbours.Remove(gameObject.name);
        }
    }

    /// <summary>
    /// Retrieve a GameObject that has been registered as interesting if it is within the agent's neighbourhood
    /// </summary>
    /// <param name="gameObject">The type of object you wish to locate in this neighbourhood</param>
    /// <returns>Returns a reference to the game object in the scene</returns>
    public GameObject RetrieveTrackedObject(GameObject gameObject)
    {
        if (gameObject == null) return null;

        if (secondaryNeighbours.ContainsKey(gameObject.name))
        {
            if(secondaryNeighbours[gameObject.name] != null)
            {
                return secondaryNeighbours[gameObject.name];
            }
        }

        return null;
    }

    public void ScanForNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, TrackingRadius);
        foreach (Collider c in colliders)
        {
            if (secondaryNeighbours.ContainsKey(c.name))
            {
                secondaryNeighbours[c.name] = c.gameObject;
            }
            
            if (toTrack == TrackType.Enemies)
            {
                Enemy e = c.GetComponent<Enemy>();
                if (e != null)
                {
                    if (neighbours.IndexOf(e) != -1)
                    {
                        neighbours.Remove(e);
                    }
                }
            }
            else if(toTrack == TrackType.Obstacles)
            {
                Transform t = c.GetComponent<Transform>();
                if (t != null)
                {
                    if (obstacles.IndexOf(t) != -1)
                    {
                        obstacles.Remove(t);
                    }
                }
            }
        }
    }
}