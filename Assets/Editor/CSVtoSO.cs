using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVtoSO
{
    private static string enemyCSVPath = "/Assets/Editor/CSVs/EnemyCSV.csv";
    private static string playerCSVPath = "/Assets/Editor/CSVs/PlayerCSV.csv";
    private static string weaponCSVPath = "/Assets/Editor/CSVs/WeaponCSV.csv";

    [MenuItem("Utilities/Generate Enemies")]
    public static void GenerateEnemies()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + enemyCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 5)
            {
                return;
            }

            Enemy enemy = ScriptableObject.CreateInstance<Enemy>();
            enemy.id = int.Parse(splitData[0]);
            enemy.enemyName = splitData[1];
            enemy.health = float.Parse(splitData[2]);
            enemy.movementSpeed = float.Parse(splitData[3]);
            enemy.damage = float.Parse(splitData[4]);

            AssetDatabase.CreateAsset(enemy, $"Assets/Resources/ScriptableObjects/Enemies/{enemy.id}.asset");
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Players")]
    public static void GeneratePlayers()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + playerCSVPath);

        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 4)
            {
                return;
            }

            Player player = ScriptableObject.CreateInstance<Player>();
            player.id = int.Parse(splitData[0]);
            player.playerName = splitData[1];
            player.health = float.Parse(splitData[2]);
            player.movementSpeed = float.Parse(splitData[3]);

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

            if (splitData.Length != 11)
            {
                return;
            }

            Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.weaponId = int.Parse(splitData[0]);
            weapon.weaponName = splitData[1];
            weapon.attackPower = float.Parse(splitData[2]);
            weapon.spritePath = splitData[3];
            weapon.thumbnailPath = splitData[4];
            weapon.weaponType = splitData[5];
            weapon.cooldown = float.Parse(splitData[6]);
            weapon.weaponRange = float.Parse(splitData[7]);
            weapon.ammoCount = int.Parse(splitData[8]);
            weapon.cost = int.Parse(splitData[9]);
            weapon.reloadSpeed = float.Parse(splitData[10]);

            AssetDatabase.CreateAsset(weapon, $"Assets/Resources/ScriptableObjects/Weapons/{weapon.weaponName}.asset");
        }

        AssetDatabase.SaveAssets();
    }

}
