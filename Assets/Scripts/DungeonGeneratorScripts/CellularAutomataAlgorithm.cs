using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public static class CellularAutomataAlgorithm
{    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static HashSet<Vector2Int> CA(Vector2Int startPosition, int roomWidth, int roomHeight, int iterations, int density, int wallRegionsThreshold, int roomRegionsThreshold)
    {

        int[,] map = MakeNoiseGrid(density, roomWidth, roomHeight);
        for (int i = 0; i < iterations; i++)
        {
            ApplyCellularAutomate(map, roomWidth, roomHeight);
        }
        HashSet<Vector2Int> room = AlgorithmUtils.MapToHashSet(startPosition,map, roomWidth, roomHeight);
        return room;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="density"></param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static int[,] MakeNoiseGrid(int density, int width, int height)
    {
        int[,] noiseGrid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (Random.Range(1, 100) > density)
                {
                    noiseGrid[x, y] = AlgorithmUtils.floorTile;
                }
                else
                {
                    noiseGrid[x, y] = AlgorithmUtils.wallTile;
                }

            }
        }
        return noiseGrid;
    }    

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static void ApplyCellularAutomate(int[,] map, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallCount = AlgorithmUtils.CountSurroundingWalls(x, y, map, width, height);

                if (neighbourWallCount > 4)
                {
                    map[x, y] = AlgorithmUtils.wallTile;
                }
                else if (neighbourWallCount < 4)
                {
                    map[x, y] = AlgorithmUtils.floorTile;
                }

            }
        }
    }

    

    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="wallRegionsThreshold"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static void FilterWalls(int wallRegionsThreshold, int[,] map, int width, int height){
         List<List<Coordinate>> wallRegions = GetRegions(AlgorithmUtils.wallTile, map, width, height);

        foreach (List<Coordinate> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallRegionsThreshold)
            {
                foreach (Coordinate tile in wallRegion)
                {
                    map[tile.x, tile.y] = AlgorithmUtils.floorTile;
                }
            }
        }
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="roomRegionsThreshold"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static List<BoundsInt> FilterRooms(int roomRegionsThreshold, int[,] map, int width, int height)
    {
        List<List<Coordinate>> roomRegions = GetRegions(AlgorithmUtils.floorTile, map, width, height);
        List<BoundsInt> finalRooms = new List<BoundsInt>();
        
        foreach (List<Coordinate> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomRegionsThreshold)
            {
                foreach (Coordinate tile in roomRegion)
                {
                    map[tile.x, tile.y] = AlgorithmUtils.wallTile;
                }
            }
            else
            {
                BoundsInt newRoom = AlgorithmUtils.CreateBoundingBox(roomRegion, width, height);
                finalRooms.Add(newRoom);
            }
        }

        return finalRooms;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
     private static List<List<Coordinate>> GetRegions(int tileType,  int[,] map, int width, int height)
    {
        List<List<Coordinate>> regions = new List<List<Coordinate>>();
        int[,] mapFalgs = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFalgs[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coordinate> newRegion = GetRegionTiles(x, y, map, width, height);
                    regions.Add(newRegion);

                    foreach (Coordinate tile in newRegion)
                    {
                        mapFalgs[tile.x, tile.y] = 1;
                    }
                }
            }
        }
        return regions;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static List<Coordinate> GetRegionTiles(int startX, int startY, int[,] map, int width, int height)
    {
        List<Coordinate> tiles = new List<Coordinate>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coordinate> queue = new Queue<Coordinate>();
        queue.Enqueue(new Coordinate(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coordinate tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x - 1; x < tile.x + 2; x++)
            {
                for (int y = tile.y - 1; y < tile.y + 2; y++)
                {
                    if (AlgorithmUtils.IsInMapRange(x, y, width, height) && (y == tile.y || x == tile.x))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coordinate(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }


}