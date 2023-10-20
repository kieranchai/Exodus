using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Assets/New Equipment")]
public class Equipment : ScriptableObject
{
    public int id;
    public string equipmentName;
    public string description;
    public string type;
    public string value;
    public string secValue;
    public int cooldown;
    public string thumbnailPath;
    public int currentCooldown;
}