using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Spawn the skeletons.
/// </summary>
public class SkeletonSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    private bool skeletonIsSpawned = false;
    public GameObject skeletonnPrefab;
    // Start is called before the first frame update    
    void Update()
    {
        if (!skeletonIsSpawned)
        {
            List<Vector3> positions = SpawnPositionGenerator.GetSkeletonPositions();
            foreach (Vector3 pos in positions)
            {   
                Instantiate(skeletonnPrefab, pos+new Vector3(0.5f,0.5f,0), Quaternion.identity);
            }
            skeletonIsSpawned = true;
        }

    }
}
