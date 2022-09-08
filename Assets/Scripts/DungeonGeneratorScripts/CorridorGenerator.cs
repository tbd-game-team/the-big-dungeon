using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorGenerator
{
    /// <summary>
    /// @author: Neele Kemper
    /// Connects each space center in the passed list with the center to which it has the shortest distance by creating coridors.
    /// </summary>
    /// <param name="roomCenters">list of room centers</param>
    /// <returns>coridors, defined as a hash set of vectors</returns>
    public static HashSet<Vector2Int> ConnectSpaces(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> cooridors = new HashSet<Vector2Int>();
        Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestCenter(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateSingleCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            cooridors.UnionWith(newCorridor);

        }
        return cooridors;
    }


    /// <summary>
    /// @author: Neele Kemper
    /// Calculates for the passed room center, the nearest center.
    /// </summary>
    /// <param name="currentRoomCenter">specific room center</param>
    /// <param name="roomCenters"list of room centers</param>
    /// <returns>position  of the closest room center.</returns>
    private static Vector2Int FindClosestCenter(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
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

    /// <summary>
    /// @author: Neele Kemper
    /// Connects the passed room center with an other room center, via a corridor.
    /// </summary>
    /// <param name="currentRoomCenter">specific room center</param>
    /// <param name="destinationCenter">target room center</param>
    /// <returns>single coridor, defined as a hash set of vectors</returns>
    private static HashSet<Vector2Int> CreateSingleCorridor(Vector2Int currentRoomCenter, Vector2Int destinationCenter)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destinationCenter.y || position.x != destinationCenter.x)
        {
            /*
            The corridor mostly runs vertically at first. 
            To make it look a little more natural, a random step is taken in a horizontal direction now and then. 
            But not too often, this would make the corridor winding, which reduces the game's fun.
            */
            if (position.y != destinationCenter.y && position.x != destinationCenter.x)
            {
                // 
                if (Random.Range(0, 100) < 80)
                {
                    position = CreateVerticalCorridor(position, destinationCenter);
                }
                else
                {
                    position = CreateHorizontalCorridor(position, destinationCenter);
                }
            }
            else if (position.y != destinationCenter.y)
            {
                position = CreateVerticalCorridor(position, destinationCenter);
            }
            else
            {
                position = CreateHorizontalCorridor(position, destinationCenter);
            }
            corridor.Add(position);
        }
        return corridor;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Creates a new corridor coordinate in the vertical direction.
    /// </summary>
    /// <param name="position">current position</param>
    /// <param name="destination">target position</param>
    /// <returns>new cooridor position </returns>
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

    /// <summary>
    /// @author: Neele Kemper
    /// Creates a new corridor coordinate in the horizontal direction.
    /// </summary>
    /// <param name="position">current position</param>
    /// <param name="destination">target position</param>
    /// <returns>new cooridor position </returns>
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
