using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Assets/New Enemy")]
public class Enemy : ScriptableObject
{
    public int id;
    public string enemyName;
    public float health;
    public float movementSpeed;
    public string equippedWeapon;
    public int xpDrop;
    public int cashDrop;
    public string lootDrop;
}
