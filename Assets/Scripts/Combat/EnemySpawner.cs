using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static void SpawnStarterEnemies()
    {
        var locations = ActorGenerator.GetEnemyPositions();

        foreach (var loc in locations)
        {
            var pathOfPrefabDirectory = "Prefabs/Combat";
            var prefabName = "Enemy";

            var prefab = Resources.Load(pathOfPrefabDirectory + prefabName, typeof(GameObject)) as GameObject;
            var enemy = Object.Instantiate(prefab, loc, Quaternion.identity);

            var enemyController = enemy.GetComponent<GenericEnemy>();
            enemyController.target = GameObject.FindWithTag("Player");


            Debug.Log("Enemy Positions: (" + loc.x + ", " + loc.y + ")");
        }
    }
}
