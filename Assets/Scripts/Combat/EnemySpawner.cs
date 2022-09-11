using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static void SpawnStarterEnemies(int[,] map, int width, int height)
    {
        var locations = SpawnPositionGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            var prefabName = "Enemy";
            var prefab = Resources.Load(prefabName);
            var enemy = Object.Instantiate(prefab, loc, Quaternion.identity) as GameObject;

            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag("Player");
            enemyController.map = map;
            enemyController.mapWidth = width;
            enemyController.mapHeight = height;


            //Debug.Log("Enemy Positions: (" + loc.x + ", " + loc.y + ")");
        }
    }
}
