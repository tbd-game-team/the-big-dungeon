using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static GameObject enemyPrefab;

    public static void SpawnStarterEnemies()
    {
        var locations = ActorGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            var enemy = Object.Instantiate(enemyPrefab);
            enemy.transform.position = loc;
            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag("Player");

            Debug.Log("Enemy Positions: (" + loc.x + ", " + loc.y + ")");
        }
    }
}
