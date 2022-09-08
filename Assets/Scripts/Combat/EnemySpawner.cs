using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    /// <summary>
    /// Spawn the initially in the level placed enemies.
    /// This information is taken from the ActorGenerator.
    /// </summary>
    public static void SpawnStarterEnemies()
    {
        var locations = ActorGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            var prefabName = "Enemy";

            SpawnEnemy(prefabName, loc);
        }
    }

    /// <summary>
    /// Spawns a single enemy. The enemy class can only be recognized if the file is put correctly.
    /// </summary>
    /// <param name="prefabName">Name of the enemy prefab resource.</param>
    /// <param name="loc">Where the enemy is to be spawned.</param>
    public static void SpawnEnemy(string prefabName, Vector3 loc)
    {
        var locations = ActorGenerator.GetEnemyPositions();

        var prefab = Resources.Load(prefabName);
        var enemy = Object.Instantiate(prefab, loc, Quaternion.identity) as GameObject;

        var enemyController = enemy.GetComponent<GenericEnemy>();
        enemyController.target = GameObject.FindWithTag("Player");
    }

}
