using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    public List<Enemy> enemiesSpawned;  //Hold reference to all enemies spawned at this point

    public GameObject enemyPrefab;  //TODO: Make list for spawning multiple types   //TODO: Research factory design pattern ?
    public int spawnAmount = 10;
    public float spawnRadius = 50f;
    public float spawnDelay = 0.1f;
    public float neighbourDistance = 5f;

    [HideInInspector] public float collisionDistance;

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

        collisionDistance = largestTransformSize * 1.5f;

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

    private void OnTriggerEnter(Collider other)
    {
            //TODO: Go more in depth on target selection behaviours
            //Maybe entering enemy spawn zone alerts them?
            //Or instead each enemy checks range?
            //Maybe alerted as a whole but don't approach until near enough
                //Can we tie in emotions? YES - increase target distance based upon emotion
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
