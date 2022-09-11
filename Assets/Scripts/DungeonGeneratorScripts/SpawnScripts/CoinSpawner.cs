using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Place the target coin.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    private bool coinIsSpawned = false;


    private void Update()
    {
        if (!coinIsSpawned)
        {
            // spawn position of player
            transform.position = SpawnPositionGenerator.GetTargetPosition()+new Vector3(0.5f,0.5f,0);
            coinIsSpawned = true;
        }
    }
}
