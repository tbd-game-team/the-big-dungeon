using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    private static readonly string[] EnemyTypes = new string[] { "Enemy", "EnemyEgg", "EnemyRanged" };

    public static void SpawnStarterEnemies(int[,] map, int width, int height)
    {
        // Map generation puts out enemy spawn locations
        var locations = SpawnPositionGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            // Determine enemy type
            string prefabName = GetRandomEnemy();
            var prefab = Resources.Load(prefabName);

            // Spawn enemy
            var enemy = Object.Instantiate(prefab, loc + new Vector3(0.5f, 0.5f, 0), Quaternion.identity) as GameObject;

            // Configure enemy
            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.map = map;
            enemyController.mapWidth = width;
            enemyController.mapHeight = height;
        }
    }

    private static string GetRandomEnemy()
    {
        return EnemyTypes[Random.Range(0, EnemyTypes.Length)];
    }
}
