using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Assets/New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public string type;
    public string primaryValue;
    public string secondaryValue;
    public int cost;
    public string thumbnailPath;
    public string inShop;
}