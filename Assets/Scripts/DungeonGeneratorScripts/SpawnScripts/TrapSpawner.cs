using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Spawn the peak traps.
/// </summary>
public class TrapSpawner : MonoBehaviour
{
    private bool trapIsSpawned = false;
    public GameObject trapPrefab;
    // Start is called before the first frame update    
    void Update()
    {
        if (!trapIsSpawned)
        {
            List<Vector3> positions = SpawnPositionGenerator.GetTrapPositions();
            foreach (Vector3 pos in positions)
            {
                Instantiate(trapPrefab, pos+new Vector3(0.5f,0.5f,0), Quaternion.identity);
            }
            trapIsSpawned = true;
        }

    }

}
