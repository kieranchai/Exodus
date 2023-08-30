using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health;
    public float movementSpeed;
    public float damage;
    public int id;
    public string enemyName;

    [SerializeField]
    private Enemy enemyData;

    private void Start()
    {
        if(enemyData)
        {
            SetEnemyData(enemyData);
        }
    }

    public void SetEnemyData(Enemy data)
    {
        health = data.health;
        movementSpeed= data.movementSpeed;
        damage = data.damage;
        id = data.id;
        enemyName= data.enemyName;
    }
}
