using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionSpawner : MonoBehaviour
{
   public GameObject healthPotionPrefab;
    // Start is called before the first frame update    
    void Start()
    {   
        List<Vector3> positions = SpawnPositionGenerator.GetHealthPotionPositions();
        foreach(Vector3 pos in positions)
        {   
            Instantiate(healthPotionPrefab, pos, Quaternion.identity);
        }
    }

}
