using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVtoSO
{
    private static string playerCSVPath = "/Assets/Editor/CSVs/Player.csv";
    private static string weaponCSVPath = "/Assets/Editor/CSVs/Weapons.csv";
    private static string itemCSVPath = "/Assets/Editor/CSVs/Items.csv";
    private static string enemyCSVPath = "/Assets/Editor/CSVs/Enemies.csv";


    [MenuItem("Utilities/Generate Player")]
    public static void GeneratePlayer()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + playerCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 8)
            {
                return;
            }

            Player player = ScriptableObject.CreateInstance<Player>();
            player.id = int.Parse(splitData[0]);
            player.playerName = splitData[1];
            player.health = float.Parse(splitData[2]);
            player.movementSpeed = float.Parse(splitData[3]);
            player.lightAmmoCap = int.Parse(splitData[4]);
            player.mediumAmmoCap = int.Parse(splitData[5]);
            player.heavyAmmoCap = int.Parse(splitData[6]);
            player.defaultWeapon = splitData[7];

            AssetDatabase.CreateAsset(player, $"Assets/Resources/ScriptableObjects/Players/{player.id}.asset");
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Weapons")]
    public static void GenerateWeapons()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + weaponCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 13)
            {
                return;
            }

            Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.id = int.Parse(splitData[0]);
            weapon.weaponName = splitData[1];
            weapon.attackPower = float.Parse(splitData[2]);
            weapon.spritePath = splitData[3];
            weapon.thumbnailPath = splitData[4];
            weapon.weaponType = splitData[5];
            weapon.cooldown = float.Parse(splitData[6]);
            weapon.weaponRange = float.Parse(splitData[7]);
            weapon.defaultAmmo = int.Parse(splitData[8]);
            weapon.cost = int.Parse(splitData[9]);
            weapon.weight = float.Parse(splitData[10]);
            weapon.ammoType = splitData[11];
            weapon.inShop = splitData[12];

            AssetDatabase.CreateAsset(weapon, $"Assets/Resources/ScriptableObjects/Weapons/{weapon.weaponName}.asset");
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Items")]
    public static void GenerateItems()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + itemCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 9)
            {
                return;
            }

            Item item = ScriptableObject.CreateInstance<Item>();
            item.id = int.Parse(splitData[0]);
            item.itemName = splitData[1];
            item.description = splitData[2];
            item.type = splitData[3];
            item.primaryValue = splitData[4];
            item.secondaryValue = splitData[5];
            item.cost = int.Parse(splitData[6]);
            item.thumbnailPath = splitData[7];
            item.inShop = splitData[8];

            AssetDatabase.CreateAsset(item, $"Assets/Resources/ScriptableObjects/Items/{item.itemName}.asset");
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Enemies")]
    public static void GenerateEnemies()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + enemyCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 8)
            {
                return;
            }

            Enemy enemy = ScriptableObject.CreateInstance<Enemy>();
            enemy.id = int.Parse(splitData[0]);
            enemy.enemyName = splitData[1];
            enemy.health = float.Parse(splitData[2]);
            enemy.movementSpeed = float.Parse(splitData[3]);
            enemy.equippedWeapon = splitData[4];
            enemy.xpDrop = int.Parse(splitData[5]);
            enemy.cashDrop = int.Parse(splitData[6]);
            enemy.lootDrop = splitData[7];

            AssetDatabase.CreateAsset(enemy, $"Assets/Resources/ScriptableObjects/Enemies/{enemy.id}.asset");
        }

        AssetDatabase.SaveAssets();
    }

}
