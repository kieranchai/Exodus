using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVtoSO
{
    private static string enemyCSVPath = "/Assets/Editor/CSVs/EnemyCSV.csv";

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

            AssetDatabase.CreateAsset(enemy, $"Assets/ScriptableObjects/Enemies/{enemy.id}.asset");
        }

        AssetDatabase.SaveAssets();
    }

}
