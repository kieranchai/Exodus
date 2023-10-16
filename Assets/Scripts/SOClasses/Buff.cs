using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Assets/New Buff")]
public class Buff : ScriptableObject
{
    public int id;
    public string buffName;
    public string description;
    public string category;
    public string type;
    public string value;
    public string secValue;
    public float rollChance;
}