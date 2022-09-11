using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Spawn the health potions.
/// </summary>
public class HealthPotionSpawner : MonoBehaviour
{
    private bool potionIsSpawned = false;
    public GameObject healthPotionPrefab;
    // Start is called before the first frame update    
    void Update()
    {
        if (!potionIsSpawned)
        {
            List<Vector3> positions = SpawnPositionGenerator.GetHealthPotionPositions();
            foreach (Vector3 pos in positions)
            {
                Instantiate(healthPotionPrefab, pos+new Vector3(0.5f,0.5f,0), Quaternion.identity);
            }
            potionIsSpawned = true;
        }

    }

}
