using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Assets/New Weapon")]
public class Weapon : ScriptableObject
{
    public int id;
    public string weaponName;
    public string description;
    public float attackPower;
    public string spritePath;
    public string thumbnailPath;
    public string weaponType;
    public float cooldown;
    public float weaponRange;
    public int clipSize;
    public float reloadSpeed;
    public int cost;
    public int currentAmmoCount;
    public string inShop;
}