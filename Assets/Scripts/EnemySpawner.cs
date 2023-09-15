using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public string spawnZone;
    public GameObject[] zoneEnemies;

    public int enemyCounter;

    public int enemyLimit = 5;
    public Collider2D currentZone;

    public float timer = 0;
    public float duration = 5f;

    void Start()
    {
        zoneEnemies = Resources.LoadAll<GameObject>($"Prefabs/Enemies/{spawnZone}");
        currentZone = gameObject.GetComponent<Collider2D>();

        for(int i = 0; i < enemyLimit; i++)
        {
            SpawnRandomEnemy();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            SpawnRandomEnemy();
            timer = 0;
        }
    }

    public void SpawnRandomEnemy()
    {
        if (enemyCounter == enemyLimit) return;
        GameObject spawnedEnemy = Instantiate(zoneEnemies[Random.Range(0, zoneEnemies.Length)], RandomPointInZone(currentZone.bounds), Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyScript>().spawnZone = currentZone.tag;
        enemyCounter++;
    }

    public Vector3 RandomPointInZone(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

}
