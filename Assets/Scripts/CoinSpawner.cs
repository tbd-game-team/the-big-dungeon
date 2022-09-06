using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    /*
    * @author: Neele Kemper
    * 
    */
   void Start()
    {
        transform.position = ActorGenerator.GetTargetPosition();
    }
}
