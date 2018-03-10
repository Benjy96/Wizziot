using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    public List<Enemy> enemiesSpawned;  //Hold reference to all enemies spawned at this point

    public GameObject enemyPrefab;  //TODO: Make list for spawning multiple types   //TODO: Research factory design pattern ?
    public int spawnAmount;
    public float spawnRadius;
    public float spawnDelay;

    public List<Vector3> spawnAreaWaypoints;
    private List<Vector3> availableSpawnPoints;

    private void Awake()
    {
        float largestTransformSize = 0f;
        if(enemyPrefab.transform.localScale.x > enemyPrefab.transform.localScale.z)
        {
            largestTransformSize = enemyPrefab.transform.localScale.x;
        }
        else
        {
            largestTransformSize = enemyPrefab.transform.localScale.z;
        }

        spawnAreaWaypoints = new List<Vector3>(spawnAmount * 2);
        availableSpawnPoints = new List<Vector3>(spawnAmount * 2);

        enemiesSpawned = new List<Enemy>();

        GenerateWaypoints();
        if (availableSpawnPoints.Count > 0)
        {
            InstantiateEnemy();
        }
    }

    private void GenerateWaypoints()
    {
        int safetyCounter = 0;

        while(spawnAreaWaypoints.Count != spawnAreaWaypoints.Capacity)
        {
            safetyCounter++;
            if (safetyCounter >= spawnRadius * spawnRadius) break;

            Vector3 randomWaypoint = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));

            //Get Colliders that aren't marked as the "Ground" (detect obstacles)
            Collider[] colliders = Physics.OverlapSphere(randomWaypoint, enemyPrefab.transform.localScale.sqrMagnitude, LayerMask.GetMask("Ground"));

            if (colliders.Length == 0 && !spawnAreaWaypoints.Contains(randomWaypoint))
            {
                spawnAreaWaypoints.Add(randomWaypoint);
                availableSpawnPoints.Add(randomWaypoint);
            }
        }
    }

    public void InstantiateEnemy()
    {
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);

        GameObject e = Instantiate(enemyPrefab, 
            availableSpawnPoints[randomIndex], 
            Quaternion.identity);

        Debug.Log(availableSpawnPoints[0]);

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
}
