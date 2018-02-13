using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    public List<Enemy> enemiesSpawned;  //Hold reference to all enemies spawned at this point

    public GameObject enemyPrefab;  //TODO: Make list for spawning multiple types   //TODO: Research factory design pattern ?
    public int spawnAmount = 10;
    public float spawnRadius = 50f;
    public float spawnDelay = 0.1f;

    public float neighbourDistance = 5f;
    public float collisionDistance;

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

        collisionDistance = largestTransformSize;

        enemiesSpawned = new List<Enemy>();
        InstantiateEnemy();
    }

    public void InstantiateEnemy()
    {
        GameObject e = Instantiate(enemyPrefab, 
            new Vector3(Random.Range(0f, neighbourDistance), 0f, Random.Range(0f, neighbourDistance)), 
            Quaternion.identity);

        Enemy enemy = e.GetComponent<Enemy>();

        enemy.Spawn = this;

        enemiesSpawned.Add(enemy);

        if (enemiesSpawned.Count < spawnAmount) Invoke("InstantiateEnemy", spawnDelay);
    }
}
