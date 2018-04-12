using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{ 
    public List<Enemy> enemiesSpawned;  //Hold reference to all enemies spawned at this point

    public GameObject enemyPrefab; 
    public int spawnAmount;
    public float spawnRadius;
    public float spawnDelay;

    public List<Vector3> spawnAreaWaypoints;
    private List<Vector3> availableSpawnPoints;

    private void Awake()
    {
        float largestTransformSize = 0f;
        if (enemyPrefab.transform.localScale.x > enemyPrefab.transform.localScale.z)
        {
            largestTransformSize = enemyPrefab.transform.localScale.x;
        }
        else
        {
            largestTransformSize = enemyPrefab.transform.localScale.z;
        }

        //Spawns with spacing - separate the spawn into chunks of size "enemy", then halved to double space between spawns
        float numWaypoints = ((spawnRadius * 2) / largestTransformSize) / 2;

        spawnAreaWaypoints = new List<Vector3>((int)numWaypoints);
        availableSpawnPoints = new List<Vector3>(spawnAreaWaypoints.Capacity);

        enemiesSpawned = new List<Enemy>();

        GenerateWaypoints();
        if (availableSpawnPoints.Count > 0)
        {
            if(GameManager.Instance.OnGameLoaded != null) GameManager.Instance.OnGameLoaded += InstantiateEnemy;
        }
    }

    private void OnDisable()
    {
        if(GameManager.Instance.OnGameLoaded != null) GameManager.Instance.OnGameLoaded -= InstantiateEnemy;
    }

    private void GenerateWaypoints()
    {
        int safetyCounter = 0;

        while (spawnAreaWaypoints.Count != spawnAreaWaypoints.Capacity)
        {
            safetyCounter++;
            if (safetyCounter >= spawnRadius * spawnRadius) break;

            Vector3 randomWaypoint = new Vector3(Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius));

            randomWaypoint = transform.position - randomWaypoint;   //convert to local co-ords about spawner

            //Get Colliders that aren't marked as the "Ground" (detect obstacles)
            Collider[] colliders = Physics.OverlapSphere(randomWaypoint, enemyPrefab.transform.localScale.sqrMagnitude, LayerMask.NameToLayer(GameMetaInfo._LAYER_GROUND_WALKABLE));

            if (colliders.Length == 0 && !spawnAreaWaypoints.Contains(randomWaypoint))
            {
                spawnAreaWaypoints.Add(randomWaypoint);
                availableSpawnPoints.Add(randomWaypoint);
            }
        }
    }

    //Recursively instantiate enemies
    public void InstantiateEnemy()
    {
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);

        GameObject e = Instantiate(enemyPrefab,
            availableSpawnPoints[randomIndex],
            Quaternion.identity);

        availableSpawnPoints.RemoveAt(randomIndex);

        Enemy enemy = e.GetComponent<Enemy>();

        enemy.Spawn = this;

        enemiesSpawned.Add(enemy);

        if (enemiesSpawned.Count < spawnAmount && availableSpawnPoints.Count > 0) Invoke("InstantiateEnemy", spawnDelay);
    }

    public void RemoveEnemy(Enemy e)
    {
        enemiesSpawned.Remove(e);
    }

    //show waypoint
    private void OnDrawGizmos()
    {
        //Spawner radius
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRadius * 2, 1f, spawnRadius * 2));

        //Spawn points
        if (spawnAreaWaypoints != null)
        {
            foreach (Vector3 wp in spawnAreaWaypoints)
            {
                Gizmos.DrawSphere(wp, 1f);
            }
        }
    }
}
