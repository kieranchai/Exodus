dusing System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public string spawnZone;
    public Enemy enemy[];
    public Enemy zoneEnemies[];

    public int enemyCounter;

    public int enemyLimit = 5;
    public Collider2D currentZone;

    void Start()
    {
        enemy[] = Resources.LoadAll<Enemy>("ScriptableObjects/Enemies");
        zoneEnemies[] = Array.Find(enemy, e => e.spawnZone == spawnZone);
        currentZone = gameObject.GetComponent<Collider2D>();
    }

    void Update()
    {
        if (enemyCounter < enemyLimit) SpawnRandomEnemy();
    }

    public void SpawnRandomEnemy()
    {
        Enemy spawnedEnemy = Instantiate(zoneEnemies[Random.Range(0, zoneEnemies.Length)], RandomPointInZone(currentZone));
        spawnedEnemy.spawnZone = currentZone.tag;
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
