using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public static class ActorGenerator
{
    private static Vector3 playerPosition = new Vector3();
    private static Vector3 targetPosition = new Vector3();
    private static List<Vector3> enemyPositions = new List<Vector3>();


    /// <summary>
    /// @author: Neele Kemper
    /// Calculate the position of the player, the target coin and the enemies.
    /// </summary>
    /// <param name="rooms">list of rooms in the dugeon.</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static void PlaceActors(List<BoundsInt> rooms, int[,] map, int width, int height)
    {
        // sort rooms ascending according to their size.
        rooms.Sort((r1, r2) => ((r1.size.x * r1.size.y).CompareTo(r2.size.x * r2.size.y)));
        List<BoundsInt> enemyRooms = new List<BoundsInt>(rooms);

        List<Vector2Int> roomCenters = AlgorithmUtils.CreateRoomCenters(rooms, map, width, height);

        // the center of the smallest room, will be the starting position of the player
        Vector2Int startPosition = roomCenters[0];
        playerPosition = new Vector3(startPosition.x, startPosition.y, 0);
        Coordinate startCoordinate = new Coordinate(startPosition.x, startPosition.y);

        roomCenters.Remove(startPosition);
        enemyRooms.Remove(rooms[0]);

        List<int> pathLengthList = new List<int>();

        // calculate the distance from the player position to any other room center
        foreach (Vector2Int center in roomCenters)
        {
            Coordinate roomCoordinate = new Coordinate(center.x, center.y);
            List<Coordinate> path = AStarAlgorithm.AStar(startCoordinate, roomCoordinate, map, width, height);
            pathLengthList.Add(path.Count);
        }

        // The room farthest from the player becomes the target room.
        int maxDistance = pathLengthList.Max();
        int targetIndex = pathLengthList.IndexOf(maxDistance);
        targetPosition = new Vector3(roomCenters[targetIndex].x, roomCenters[targetIndex].y, 0);

        // enemies are placed in all other rooms.
        enemyRooms.RemoveAt(targetIndex);
        pathLengthList.RemoveAt(targetIndex);
        CalculateEnemyPositions(enemyRooms, pathLengthList, map, width, height);

    }

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate the position of the enemie using the distance from the player positions and the size of the room.
    /// The farther the room is from the starting position, the more enemies will be spawned in that room.
    /// </summary>
    /// <param name="rooms">list of rooms in the dugeon.</param>
    /// <param name="pathLengths">List of distances of the rooms from the player position.</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static void CalculateEnemyPositions(List<BoundsInt> rooms, List<int> pathLengths, int[,] map, int width, int height)
    {
        int nSteps = 3;
        int stepsWide = (pathLengths.Max() - pathLengths.Min()) / nSteps;

        // calculate the number of enemies and their position for each room.
        for (int i = 0; i < rooms.Count; i++)
        {
            BoundsInt room = rooms[i];
            int length = pathLengths[i];
            float enemyDensity = 0.0f;
            // the further the room is from the starting position, the more enemies will be spawned (enemy density increases).
            if (length <= stepsWide)
            {
                enemyDensity = 0.03f;
            }
            else if (length <= 2 * stepsWide)
            {
                enemyDensity = 0.06f;
            }
            else
            {
                enemyDensity = 0.09f;
            }
            // calculate number of enemies
            int roomArea = (room.size.x * room.size.y) / 2;
            int nEnemies = Mathf.Max(1, (int)(roomArea * enemyDensity));

            int count = 0;

            // calculate the position for each enemy
            while (count < nEnemies)
            {
                // randomly select the position of the enemy in the room.
                int x = Random.Range(room.min.x, room.max.x);
                int y = Random.Range(room.min.y, room.max.y);
                if (AlgorithmUtils.IsInMapRange(x, y, width, height))
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    // do not place enemies directly against walls (this may cause them to overlap with the walls)
                    int surroundingWalls = AlgorithmUtils.CountSurroundingWalls(x, y, map, width, height);
                    bool isFloor = (map[x, y] == AlgorithmUtils.floorTile);
                    if (isFloor && !enemyPositions.Contains(pos) && surroundingWalls < 2)
                    {
                        enemyPositions.Add(pos);
                        count++;

                    }
                }

            }
        }
    }


    /// <summary>
    /// @author: Neele Kemper
    /// Get the player's spawn position.
    /// </summary>
    /// <returns>spawn position</returns>
    public static Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }


    /// <summary>
    /// @author: Neele Kemper
    /// Get the coin's spawn position.
    /// </summary>
    /// <returns>coin position</returns>
    public static Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Get the spawn positions of the enemies.
    /// </summary>
    /// <returns>enemy positions</returns>
    public static List<Vector3> GetEnemyPositions()
    {
        return enemyPositions;
    }
}

