using System.Collections.Generic;
using UnityEngine;

public static class AlgorithmUtils
{
    // a wall tile is defined as 1 in the dungeon map
    public static int wallTile = 1;
    // a floor tile is defined as 0 in the dungeon map
    public static int floorTile = 0;


    /// <summary>
    /// @author: Neele Kemper
    /// Convert the dungeon map into a hash set of vectors.
    /// </summary>
    /// <param name="startPosition">position of the lower left corner of the map</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>dungeon defined as hash set of vectors</returns>
    public static HashSet<Vector2Int> MapToHashSet(Vector2Int startPosition, int[,] map, int width, int height)
    {
        HashSet<Vector2Int> room = new HashSet<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == floorTile)
                {
                    int xPos = (int)x + startPosition.x;
                    int yPos = (int)y + startPosition.y;
                    var newFloor = new Vector2Int(xPos, yPos);
                    room.Add(newFloor);
                }
            }
        }

        return room;
    }


    /// <summary>
    /// @author: Neele Kemper
    /// Convert the dungeon hash set into a binary map
    /// </summary>
    /// <param name="hasSet">the dungeon hash set</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static int[,] HashSetToMap(HashSet<Vector2Int> hasSet, int width, int height)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                map[x, y] = wallTile;

            }
        }
        foreach (Vector2Int room in hasSet)
        {
            if (IsInMapRange(room.x, room.y, width, height))
            {
                map[room.x, room.y] = floorTile;
            }

        }
        return map;
    }


    /// <summary>
    /// @author: Neele Kemper
    /// Check if a coordinate is in the range of the dungeon map.
    /// </summary>
    /// <param name="x"x x-coordinate></param>
    /// <param name="y">y-coordinate</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>true if the coordinate is in the map, false otherwise.</returns>
    public static bool IsInMapRange(int x, int y, int width, int height)
    {
        return x > -1 && x < width && y > -1 && y < height;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Create a bounding box around a cellular room. 
    /// </summary>
    /// <param name="rooms">cellular room</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>bounding box</returns>
    public static BoundsInt CreateBoundingBox(List<Coordinate> rooms, int width, int height)
    {
        Vector2Int bottomLeftCornerMin = new Vector2Int(width - 1, height - 1);
        Vector2Int upperRightCornerMax = new Vector2Int(0, 0);

        foreach (Coordinate c in rooms)
        {

            if (c.x < bottomLeftCornerMin.x)
            {
                bottomLeftCornerMin = new Vector2Int(c.x, bottomLeftCornerMin.y);
            }
            if (c.y < bottomLeftCornerMin.y)
            {
                bottomLeftCornerMin = new Vector2Int(bottomLeftCornerMin.x, c.y);
            }
            if (c.x > upperRightCornerMax.x)
            {
                upperRightCornerMax = new Vector2Int(c.x, upperRightCornerMax.y);
            }
            if (c.y > upperRightCornerMax.y)
            {
                upperRightCornerMax = new Vector2Int(upperRightCornerMax.x, c.y);
            }
        }

        Vector2Int bottomLeftCorner = bottomLeftCornerMin + new Vector2Int(-1, -1);
        int sizeX = (upperRightCornerMax.x - bottomLeftCornerMin.x) + 2;
        int sizeY = (upperRightCornerMax.y - bottomLeftCornerMin.y) + 2;
        Vector2Int size = new Vector2Int(sizeX, sizeY);

        BoundsInt newRoom = new BoundsInt((Vector3Int)bottomLeftCorner, (Vector3Int)size);
        return newRoom;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate for each room, the center.
    /// </summary>
    /// <param name="roomsList">list of rooms</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>list of room centers</returns>
    public static List<Vector2Int> CreateRoomCenters(List<BoundsInt> roomsList, int[,] map, int width, int height)
    {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (BoundsInt room in roomsList)
        {
            bool isFloor = false;
            Vector2Int position = (Vector2Int)Vector3Int.RoundToInt(room.center);
            Vector2Int randomDirection = new Vector2Int(0, 0);
            while (!isFloor)
            {
                if (AlgorithmUtils.IsInMapRange(position.x + randomDirection.x, position.y + randomDirection.y, width, height))
                {
                    position = position + randomDirection;
                    if (map[position.x, position.y] == AlgorithmUtils.floorTile)
                    {
                        isFloor = true;
                        roomCenters.Add(position);
                    }
                    else
                    {
                        // If the center of the room is not a floor tile, search for a floor tile near the center and select it as the center.
                        randomDirection = new Vector2Int(Random.Range(-1, 1), Random.Range(-1, 1));

                    }
                }
                else
                {
                    break;
                }
            }
        }
        return roomCenters;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Count the number of surrounded walls for the passed coordinate.
    /// </summary>
    /// <param name="x"x x-coordinate></param>
    /// <param name="y">y-coordinate</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>number of surrounding walls</returns>
    public static int CountSurroundingWalls(int x, int y, int[,] map, int width, int height)
    {
        int wallCount = 0;
        // 3x3 Grid
        for (int w = x - 1; w < x + 2; w++)
        {
            for (int h = y - 1; h < y + 2; h++)
            {
                if (AlgorithmUtils.IsInMapRange(w, h, width, height))
                {
                    if (!(w == x && h == y)) // Do not add center in
                    {
                        wallCount += map[w, h];
                    }

                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

}