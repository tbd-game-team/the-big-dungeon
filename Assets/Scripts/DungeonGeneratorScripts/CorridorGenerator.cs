using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorGenerator
{       
    /*
    * @author: Neele Kemper
    * 
    */
    public static HashSet<Vector2Int> ConnectSpaces(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> cooridors = new HashSet<Vector2Int>();
        Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPoint(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateStraightCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            cooridors.UnionWith(newCorridor);

        }
        return cooridors;
    }
    
        
    /*
    * @author: Neele Kemper
    * 
    */
    private static Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {

        Vector2Int closest = new Vector2Int();
        float distance = float.MaxValue;
        foreach (Vector2Int position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }
        
    /*
    * @author: Neele Kemper
    * 
    */
    private static HashSet<Vector2Int> CreateStraightCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = currentRoomCenter;
        corridor.Add(position);
        
        /*
        while (position.y != destination.y)
        {
            position = CreateVerticalCorridor(position, destination);
            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            position= CreateHorizontalCorridor(position, destination);
            corridor.Add(position);

        }*/

        
        
        while (position.y != destination.y || position.x != destination.x)
        {   
           
            if (position.y != destination.y && position.x != destination.x)
            {   
                if (Random.Range(0, 100) < 85)
                {
                    position = CreateVerticalCorridor(position, destination);
                }
                else
                {
                    position= CreateHorizontalCorridor(position, destination);
                }
            }
            else if (position.y != destination.y)
            {
                position = CreateVerticalCorridor(position, destination);
            }
            else
            {
                position= CreateHorizontalCorridor(position, destination);
            }
            corridor.Add(position);
        }
        return corridor;

    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    private static Vector2Int CreateVerticalCorridor(Vector2Int position, Vector2 destination)
    {
        if (destination.y > position.y)
        {
            position += Vector2Int.up;
        }
        else if (destination.y < position.y)
        {
            position += Vector2Int.down;
        }
        return position;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    private static Vector2Int CreateHorizontalCorridor(Vector2Int position, Vector2 destination)
    {
        if (destination.x > position.x)
        {
            position += Vector2Int.right;
        }
        else if (destination.x < position.x)
        {
            position += Vector2Int.left;
        }
        
        return position;
    }





}
