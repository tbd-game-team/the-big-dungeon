using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

public static class EnemySpawner
{
    private static readonly string[] EnemyTypes = new string[] { "EnemyEgg" };

    private static Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();

    /// <summary>
    /// Loads prefab from disk. The enemy class can only be recognized if the file is put correctly.
    /// </summary>
    private static GameObject GetPrefab(string prefabName)
    {
        if (_prefabCache.TryGetValue(prefabName, out GameObject value))
        {
            return value;
        }

        var prefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
        _prefabCache[prefabName] = prefab;

        return prefab;
    }

    /// <summary>
    /// Spawn the initially in the level placed enemies.
    /// This information is taken from the ActorGenerator.
    /// </summary>
    public static void SpawnStarterEnemies(int[,] map, int width, int height)
    {
        // Map generation puts out enemy spawn locations
        var locations = SpawnPositionGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            // Determine enemy type
            string prefabName = GetRandomEnemy();

            SpawnEnemy(prefabName, loc, map, width, height);
        }
    }

    /// <summary>
    /// Spawns a single enemy.
    /// </summary>
    /// <param name="prefabName">Name of the enemy prefab resource.</param>
    /// <param name="loc">Where the enemy is to be spawned.</param>
    private static void SpawnEnemy(string prefabName, Vector3 loc, int[,] map, int width, int height)
    {
        var prefab = GetPrefab(prefabName);

        // Spawn enemy
        var enemy = Object.Instantiate(prefab, loc + new Vector3(0.5f, 0.5f, 0), Quaternion.identity) as GameObject;

        // Configure enemy
        var enemyController = enemy.GetComponent<GenericEnemy>();
        enemyController.map = map;
        enemyController.mapWidth = width;
        enemyController.mapHeight = height;
    }

    /// <summary>
    /// Randomly selects enemy type
    /// </summary>
    /// <returns>Enemy type prefab identifier</returns>
    private static string GetRandomEnemy()
    {
        return EnemyTypes[Random.Range(0, EnemyTypes.Length)];
    }
}
