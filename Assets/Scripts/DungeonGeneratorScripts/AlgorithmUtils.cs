using System.Collections.Generic;
using UnityEngine;

public static class AlgorithmUtils
{
    public static int wallTile = 1;
    public static int floorTile = 0;


        
    /*
    * @author: Neele Kemper
    * 
    */
    public static HashSet<Vector2Int> MapToHashSet(int[,] map, Vector2Int startPosition, int width, int height)
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

    
    /*
    * @author: Neele Kemper
    * 
    */
    public static int[,] HashSetToMap(HashSet<Vector2Int> rooms, int dungeonWidth, int dungeonHeight)
    {
        int[,] map = new int[dungeonWidth, dungeonHeight];
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {

                map[x, y] = wallTile;

            }
        }
        foreach (Vector2Int room in rooms)
        {
            if (IsInMapRange(room.x, room.y, dungeonWidth, dungeonHeight))
            {
                map[room.x, room.y] = floorTile;
            }

        }
        return map;
    }

    
    /*
    * @author: Neele Kemper
    * 
    */
    public static bool IsInMapRange(int x, int y, int width, int height)
    {
        return x > -1 && x < width && y > -1 && y < height;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public static BoundsInt CreateBoundingBox(List<Coordinate> rooms, int width, int height)
    {
        Vector2Int bottomLeftCornerMin = new Vector2Int(width - 1, height - 1);
        Vector2Int upperRightCornerMax = new Vector2Int(0, 0);

        foreach (Coordinate c in rooms)
        {

            if (c.tileX < bottomLeftCornerMin.x)
            {
                bottomLeftCornerMin = new Vector2Int(c.tileX, bottomLeftCornerMin.y);
            }
            if (c.tileY < bottomLeftCornerMin.y)
            {
                bottomLeftCornerMin = new Vector2Int(bottomLeftCornerMin.x, c.tileY);
            }
            if (c.tileX > upperRightCornerMax.x)
            {
                upperRightCornerMax = new Vector2Int(c.tileX, upperRightCornerMax.y);
            }
            if (c.tileY > upperRightCornerMax.y)
            {
                upperRightCornerMax = new Vector2Int(upperRightCornerMax.x, c.tileY);
            }
        }

        Vector2Int bottomLeftCorner = bottomLeftCornerMin + new Vector2Int(-1, -1);
        int sizeX = (upperRightCornerMax.x - bottomLeftCornerMin.x) + 2;
        int sizeY = (upperRightCornerMax.y - bottomLeftCornerMin.y) + 2;
        Vector2Int size = new Vector2Int(sizeX, sizeY);

        BoundsInt newRoom = new BoundsInt((Vector3Int)bottomLeftCorner, (Vector3Int)size);
        return newRoom;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public static List<Vector2Int> CreateRoomCenters(List<BoundsInt> roomsList, int width, int height, int[,] map)
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

}