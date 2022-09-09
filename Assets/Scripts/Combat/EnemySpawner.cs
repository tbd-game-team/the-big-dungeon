using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static void SpawnStarterEnemies()
    {
        var locations = SpawnPositionGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            var prefabName = "Enemy";
            var prefab = Resources.Load(prefabName);
            var enemy = Object.Instantiate(prefab, loc, Quaternion.identity) as GameObject;

            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag("Player");


            //Debug.Log("Enemy Positions: (" + loc.x + ", " + loc.y + ")");
        }
    }
}
