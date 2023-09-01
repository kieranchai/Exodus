using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Assets/New Player")]
public class Player : ScriptableObject
{
    public int id;
    public string playerName;
    public float health;
    public float movementSpeed;
    public int lightAmmoCap;
    public int mediumAmmoCap;
    public int heavyAmmoCap;
    public string defaultWeapon;
}
