using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Place the target coin.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    void Start()
    {
        transform.position = ActorGenerator.GetTargetPosition();
    }
}
