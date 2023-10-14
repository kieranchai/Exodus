using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public string spawnZone;
    public GameObject[] zoneEnemies;
    public bool canRespawn = true;
    public int enemyCounter;

    public int enemyLimit = 5;
    public Collider2D currentZone;

    public float timer = 0;
    public float duration = 5f;

    private int random;
    private int cumulative;
    private GameObject chosenEnemy;

    //For Shop Access
    public bool zoneUnlocked = false;
    public bool hasTurret = false;

    void Start()
    {
        if (!hasTurret)
        {
            zoneEnemies = Resources.LoadAll<GameObject>($"Prefabs/Enemies/{spawnZone}");
            Array.Sort(zoneEnemies, (a, b) => a.GetComponent<EnemyScript>()._data.spawnChance - b.GetComponent<EnemyScript>()._data.spawnChance);
            currentZone = gameObject.GetComponent<Collider2D>();

            for (int i = 0; i < enemyLimit; i++)
            {
                SpawnRandomEnemy();
            }

        }

        currentZone = gameObject.GetComponent<Collider2D>();
    }

    void Update()
    {
            if (canRespawn)
            {
                timer += Time.deltaTime;
                if (timer >= duration)
                {
                    SpawnRandomEnemy();
                    timer = 0;
                }
            }

        if (!zoneUnlocked)
        {
            zoneUnlocked = CheckIfZoneUnlocked();
        }
    }

    private bool CheckIfZoneUnlocked()
    {
        if (enemyCounter > 0) return false;
        if (!GameController.instance.tutorialFlag2) GameController.instance.tutorialFlag2 = true;
        //TODO: add effects when unlocked, can add heal over time when unlocked ontrigger
        return true;
    }

    public void SpawnRandomEnemy()
    {
        if (enemyCounter == enemyLimit) return;
        Vector3 spawnLocation = RandomPointInZone(currentZone.bounds);

        random = Random.Range(1, 100);
        cumulative = 0;

        for (int i = 0; i < zoneEnemies.Length; i++)
        {
            cumulative += zoneEnemies[i].GetComponent<EnemyScript>()._data.spawnChance;
            if (random < cumulative)
            {
                chosenEnemy = zoneEnemies[i];
                break;
            }
        }

        GameObject spawnedEnemy = Instantiate(chosenEnemy, spawnLocation, Quaternion.identity);
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
