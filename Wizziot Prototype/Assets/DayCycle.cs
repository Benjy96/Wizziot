using System;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour {

    public List<EnemySpawnPoint> spawners;

    public float dayDuration = 30f;
    private float time;

    // Update is called once per frame
    void Update () {
        time += Time.deltaTime / dayDuration;
        if(time >= 1)
        {
            time = 0f;
        }

        transform.localRotation = Quaternion.Euler(time * 360f, -50f, 0f);

        if(transform.localEulerAngles.x > 180f)
        {
            Debug.Log("Scaring");
            ScareAllEnemies();   
        }
	}

    private void ScareAllEnemies()
    {
        foreach (EnemySpawnPoint spawn in spawners)
        {
            foreach (Enemy e in spawn.enemiesSpawned)
            {
                e.Influence(Emotion.Fear, .25f * Time.deltaTime);
            }
        }
    }
}
