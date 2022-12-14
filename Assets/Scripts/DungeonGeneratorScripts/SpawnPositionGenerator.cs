using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SpawnPositionGenerator
{
    private static Vector3 playerPosition = new Vector3();
    private static Vector3 targetPosition = new Vector3();
    private static List<Vector3> healthPotionPositions = new List<Vector3>();
    private static List<Vector3> skeletonPositions = new List<Vector3>();
    private static List<Vector3> enemyPositions = new List<Vector3>();
    private static List<Vector3> trapPositions = new List<Vector3>();

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate the position of the player, the target coin, helath potions (and skeleton), traps and the enemies.
    /// </summary>
    /// <param name="dungeonCorridors">the dungeon corridors connecting the rooms</param>
    /// <param name="rooms">list of rooms in the dugeon.</param>
    /// <param name="healthPotionProbability">the chance that a helath potion is spawned</param>
    /// <param name="trapProbability">the chance that a trap is spawned</param>
    /// <param name="enemyDenisityLevels">indicates the density in which the enemies are spawned in levels</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static void CalculatePositions(List<BoundsInt> rooms, HashSet<Vector2Int> dungeonCorridors, int healthPotionProbability, int trapProbability, float[] enemyDenisityLevels, int[,] map, int width, int height)
    {
        // sort rooms ascending according to their size.
        rooms.Sort((r1, r2) => ((r1.size.x * r1.size.y).CompareTo(r2.size.x * r2.size.y)));
        List<BoundsInt> enemyRooms = new List<BoundsInt>(rooms);

        List<Vector2Int> roomCenters = AlgorithmUtils.CreateRoomCenters(rooms, map, width, height);

        // the center of the smallest room, will be the starting position of the player
        Vector2Int startPosition = roomCenters[0];
        playerPosition = new Vector3(startPosition.x, startPosition.y, 0);

        roomCenters.Remove(startPosition);
        enemyRooms.Remove(rooms[0]);

        List<int> pathLengthList = new List<int>();

        // calculate the distance from the player position to any other room center
        foreach (Vector2Int center in roomCenters)
        {
            Coordinate roomCoordinate = new Coordinate(center.x, center.y);
            List<Coordinate> path = AStarAlgorithm.AStar(new Coordinate(playerPosition), roomCoordinate, map, width, height);
            pathLengthList.Add(path.Count);
        }

        // The room farthest from the player becomes the target room.
        int maxDistance = pathLengthList.Max();
        int targetIndex = pathLengthList.IndexOf(maxDistance);
        targetPosition = new Vector3(roomCenters[targetIndex].x, roomCenters[targetIndex].y, 0);

        // enemies and health potions are placed in all other rooms.
        enemyRooms.RemoveAt(targetIndex);
        pathLengthList.RemoveAt(targetIndex);

        CalculatePrefabPositions(enemyRooms, pathLengthList, healthPotionProbability, enemyDenisityLevels, map, width, height);

        CalculateTrapPositions(dungeonCorridors, trapProbability, map, width, height);
    }

    /// <summary>
    /// @author: Neele Kemper
    /// The positions for the enemies and for the health potions are calculated.
    /// Enemy: The position of the enemies  are calculated using the distance from the player positions and the size of the room.  The farther the room is from the starting position, the more enemies will be spawned in that room.
    /// Health potion: to a certain probability (healthPotionProbability), a health potion (and skeleton) is placed in a room.
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
        for (int i = 0; i < pathLengths.Count; i++)
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

                        // place a skeleton next to each health potion
                        bool skeletonPositionFound = false;
                        while (!skeletonPositionFound)
                        {
                            Vector2Int randomDirection = Direction2D.eightDirectionsList[Random.Range(0, Direction2D.eightDirectionsList.Count)];
                            Vector2Int newskeletonPosition =  new Vector2Int((int)newPosition.x, (int)newPosition.y) + randomDirection;

                            bool inMapRange = AlgorithmUtils.IsInMapRange(newskeletonPosition.x, newskeletonPosition.y, width, height);
                            if (inMapRange)
                            {
                                bool isFloor = (map[newskeletonPosition.x, newskeletonPosition.y] == AlgorithmUtils.floorTile);
                                if (isFloor)
                                {
                                    skeletonPositions.Add(new Vector3(newskeletonPosition.x, newskeletonPosition.y,0));
                                    skeletonPositionFound = true;
                                }

                            }
                        }
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
            //int surroundingWalls = AlgorithmUtils.CountSurroundingWalls(x, y, map, width, height);
            bool isFloor = (map[x, y] == AlgorithmUtils.floorTile);
            if (isFloor && !enemyPositions.Contains(pos) && !healthPotionPositions.Contains(pos))
            {
                return pos;

            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Traps are placed randomly in corridors
    /// </summary>
    /// <param name="dungeonCorridors">the dungeon corridors connecting the rooms</param>
    /// <param name="trapProbability">the chance that a trap is spawned</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>

    private static void CalculateTrapPositions(HashSet<Vector2Int> dungeonCorridors, int trapProbability, int[,] map, int width, int height)
    {
        foreach (Vector2Int corridor in dungeonCorridors)
        {
            int surroundingWalls = AlgorithmUtils.CountSurroundingWalls(corridor.x, corridor.y, map, width, height);
            if (surroundingWalls == 6 && Random.Range(1, 100) < trapProbability)
            {
                trapPositions.Add(new Vector3(corridor.x, corridor.y, 0));
            }
        }
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
        skeletonPositions = new List<Vector3>();
        enemyPositions = new List<Vector3>();
        trapPositions = new List<Vector3>();
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


    /// <summary>
    /// @author: Neele Kemper
    /// Get the spawn positions of the traps.
    /// </summary>
    /// <returns>enemy positions</returns>
    public static List<Vector3> GetTrapPositions()
    {
        return trapPositions;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Get the spawn positions of the skeletons.
    /// </summary>
    /// <returns>skeleton positions</returns>
    public static List<Vector3> GetSkeletonPositions()
    {
        return skeletonPositions;
    }
}
