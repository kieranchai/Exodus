using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Assets/New Enemy")]
public class Enemy : ScriptableObject
{
    public int id;
    public string enemyName;
    public float health;
    public float movementSpeed;
    public float damage;
}
