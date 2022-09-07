using System.Collections.Generic;
using UnityEngine;

public static class AlgorithmUtils
{
    public static int wallTile = 1;
    public static int floorTile = 0;


    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="rooms"></param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static int[,] HashSetToMap(HashSet<Vector2Int> rooms, int width, int height)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                map[x, y] = wallTile;

            }
        }
        foreach (Vector2Int room in rooms)
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
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static bool IsInMapRange(int x, int y, int width, int height)
    {
        return x > -1 && x < width && y > -1 && y < height;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="rooms"></param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="roomsList"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
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