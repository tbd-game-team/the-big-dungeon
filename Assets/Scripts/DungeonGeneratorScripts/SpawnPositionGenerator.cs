using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SpawnPositionGenerator
{
    private static Vector3 playerPosition = new Vector3();
    private static Vector3 targetPosition = new Vector3();
    private static List<Vector3> healthPotionPositions = new List<Vector3>();
    private static List<Vector3> enemyPositions = new List<Vector3>();

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate the position of the player, the target coin, helath potions and the enemies.
    /// </summary>
    /// <param name="rooms">list of rooms in the dugeon.</param>
    /// <param name="healthPotionProbability">the chance that a helath potion is spawned</param>
    /// <param name="enemyDenisityLevels">indicates the density in which the enemies are spawned in levels</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static void CalculatePositions(List<BoundsInt> rooms, int healthPotionProbability, float[] enemyDenisityLevels, int[,] map, int width, int height)
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

        CalculatePrefabPositions(enemyRooms, pathLengthList, healthPotionProbability, enemyDenisityLevels, map, width, height);
        EnemySpawner.SpawnStarterEnemies();
    }

    /// <summary>
    /// @author: Neele Kemper
    /// The positions for the enemies and for the health potions are calculated.
    /// Enemy: The position of the enemies  are calculated using the distance from the player positions and the size of the room.  The farther the room is from the starting position, the more enemies will be spawned in that room.
    /// Health potion: to a certain probability (healthPotionProbability), a health potion is placed in a room.
    /// </summary>
    /// <param name="rooms">list of rooms in the dugeon.</param>
    /// <param name="pathLengths">List of distances of the rooms from the player position.</param>
    /// <param name="healthPotionProbability">the chance that a helath potion is spawned</param>
    /// <param name="enemyDenisityLevels">indicates the density in which the enemies are spawned in levels</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static void CalculatePrefabPositions(List<BoundsInt> rooms, List<int> pathLengths, int healthPotionProbability, float[] enemyDenisityLevels, int[,] map, int width, int height)
    {
        int nLevel = enemyDenisityLevels.Length;
        int nLevelWidth = (pathLengths.Max() - pathLengths.Min()) / nLevel;

        // calculate the number of enemies and their position for each room.
        for (int i = 0; i < rooms.Count; i++)
        {
            BoundsInt room = rooms[i];
            int length = pathLengths[i];

            float enemyDensity = 0.0f;
            // the further the room is from the starting position, the more enemies will be spawned (enemy density increases).
            for (int j = 1; j <= nLevel; j++)
            {
                if (length <= j * nLevelWidth)
                {
                    enemyDensity = enemyDenisityLevels[j - 1];
                    break;
                }
            }

            // calculate number of enemies
            int roomArea = (room.size.x * room.size.y) / 2;
            int nEnemies = Mathf.Max(1, (int)(roomArea * enemyDensity));

            int enemyCount = 0;
            // calculate the position for each enemy
            while (enemyCount < nEnemies)
            {
                Vector3 newPosition = GetRandomPositionInRoom(room, map, width, height);
                if (newPosition != Vector3.zero)
                {
                    enemyPositions.Add(newPosition);
                    enemyCount++;

                }

            }


            // randomly determine if a health potion is placed in a random position in the room..
            if (Random.Range(1, 100) < healthPotionProbability)
            {
                bool positionFound = false;
                while (!positionFound)
                {
                    Vector3 newPosition = GetRandomPositionInRoom(room, map, width, height);
                    if (newPosition != Vector3.zero)
                    {
                        healthPotionPositions.Add(newPosition);
                        positionFound = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate a random position for a prefab in the passed room.
    /// </summary>
    /// <param name="room">room in which a prefab is to be placed.</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>position of the prefabs</returns>
    private static Vector3 GetRandomPositionInRoom(BoundsInt room, int[,] map, int width, int height)
    {
        int x = Random.Range(room.min.x, room.max.x);
        int y = Random.Range(room.min.y, room.max.y);
        if (AlgorithmUtils.IsInMapRange(x, y, width, height))
        {
            Vector3 pos = new Vector3(x, y, 0);
            // do not place prefabs directly next to the wall. (this may cause them to overlap with the walls)
            int surroundingWalls = AlgorithmUtils.CountSurroundingWalls(x, y, map, width, height);
            bool isFloor = (map[x, y] == AlgorithmUtils.floorTile);
            if (isFloor && !enemyPositions.Contains(pos) && !healthPotionPositions.Contains(pos) && surroundingWalls < 2)
            {
                return pos;

            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Reset all positions.
    /// </summary>
    /// <returns></returns>
    public static void Clear()
    {
        playerPosition = new Vector3();
        targetPosition = new Vector3();
        healthPotionPositions = new List<Vector3>();
        enemyPositions = new List<Vector3>();
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

    /// <summary>
    /// @author: Neele Kemper
    /// Get the spawn positions of the health potions.
    /// </summary>
    /// <returns>enemy positions</returns>
    public static List<Vector3> GetHealthPotionPositions()
    {
        return healthPotionPositions;
    }
}
