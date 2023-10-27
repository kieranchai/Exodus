using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MothershipSpawnsScript : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float enemyCounter = 0;
    public Collider2D currentZone;

    private GameObject chosenEnemy;

    void Start()
    {
        currentZone = gameObject.GetComponent<Collider2D>();
    }

    public void SpawnRandomEnemies(int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnRandomEnemy();
        }
    }

    public void SpawnMagnetHereticCombo(int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnMagnet();
            SpawnHeretic();
        }
    }

    public void SpawnSplitters(int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnSplitter();
        }
    }

    public void SpawnRandomEnemy()
    {
        Vector3 spawnLocation = RandomPointInZone(currentZone.bounds);
        GameObject spawnedEnemy = null;
        bool notChosen = true;
        while (notChosen)
        {
            chosenEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count - 1)];
            if (chosenEnemy.name.Contains("Golden"))
            {
                if (Random.value <= 0.05f) notChosen = false;
            }
            else
            {
                notChosen = false;
            }
        }
        spawnedEnemy = Instantiate(chosenEnemy, spawnLocation, Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyScript>().spawnZone = currentZone.tag;
        spawnedEnemy.GetComponent<EnemyScript>().currentState = EnemyScript.ENEMY_STATE.CHASE;
        enemyCounter++;
    }

    public void SpawnSplitter()
    {
        Vector3 spawnLocation = RandomPointInZone(currentZone.bounds);
        GameObject spawnedEnemy = null;
        chosenEnemy = enemyPrefabs.Find(a => a.name.Contains("Splitter"));
        spawnedEnemy = Instantiate(chosenEnemy, spawnLocation, Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyScript>().spawnZone = currentZone.tag;
        spawnedEnemy.GetComponent<EnemyScript>().currentState = EnemyScript.ENEMY_STATE.CHASE;
        enemyCounter++;
    }

    public void SpawnMagnet()
    {
        Vector3 spawnLocation = RandomPointInZone(currentZone.bounds);
        GameObject spawnedEnemy = null;
        chosenEnemy = enemyPrefabs.Find(a => a.name.Contains("Magnet"));
        spawnedEnemy = Instantiate(chosenEnemy, spawnLocation, Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyScript>().spawnZone = currentZone.tag;
        spawnedEnemy.GetComponent<EnemyScript>().currentState = EnemyScript.ENEMY_STATE.CHASE;
        enemyCounter++;
    }

    public void SpawnHeretic()
    {
        Vector3 spawnLocation = RandomPointInZone(currentZone.bounds);
        GameObject spawnedEnemy = null;
        chosenEnemy = enemyPrefabs.Find(a => a.name.Contains("Heretic"));
        spawnedEnemy = Instantiate(chosenEnemy, spawnLocation, Quaternion.identity);
        spawnedEnemy.GetComponent<EnemyScript>().spawnZone = currentZone.tag;
        spawnedEnemy.GetComponent<EnemyScript>().currentState = EnemyScript.ENEMY_STATE.CHASE;
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
