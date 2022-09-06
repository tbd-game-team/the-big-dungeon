using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public static class CellularAutomataAlgorithm
{    
    /*
    * @author: Neele Kemper
    * 
    */
    public static HashSet<Vector2Int> CA(Vector2Int startPosition, int roomWidth, int roomHeight, int iterations, int density, int wallRegionsThreshold, int roomRegionsThreshold)
    {

        int[,] map = MakeNoiseGrid(roomWidth, roomHeight, density);
        for (int i = 0; i < iterations; i++)
        {
            ApplyCellularAutomate(roomWidth, roomHeight, map);
        }
        HashSet<Vector2Int> room = AlgorithmUtils.MapToHashSet(map, startPosition, roomWidth, roomHeight);
        return room;
    }

    private static int[,] MakeNoiseGrid(int width, int height, int density)
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

    /*
    * @author: Neele Kemper
    * 
    */
    private static void ApplyCellularAutomate(int width, int height, int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallCount = CountSurroundingWalls(x, y, width, height, map);

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

    
    /*
    * @author: Neele Kemper
    * 
    */
    public static int CountSurroundingWalls(int x, int y, int width, int height, int[,] map)
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
    
    /*
    * @author: Neele Kemper
    * 
    */
    public static void FilterWalls(int wallRegionsThreshold, int width, int height, int[,] map){
         List<List<Coordinate>> wallRegions = GetRegions(AlgorithmUtils.wallTile, width, height, map);

        foreach (List<Coordinate> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallRegionsThreshold)
            {
                foreach (Coordinate tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = AlgorithmUtils.floorTile;
                }
            }
        }
    }

    public static List<BoundsInt> FilterRooms(int roomRegionsThreshold, int width, int height, int[,] map)
    {
        List<List<Coordinate>> roomRegions = GetRegions(AlgorithmUtils.floorTile, width, height, map);
        List<BoundsInt> finalRooms = new List<BoundsInt>();
        
        foreach (List<Coordinate> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomRegionsThreshold)
            {
                foreach (Coordinate tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = AlgorithmUtils.wallTile;
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

     private static List<List<Coordinate>> GetRegions(int tileType, int width, int height, int[,] map)
    {
        List<List<Coordinate>> regions = new List<List<Coordinate>>();
        int[,] mapFalgs = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFalgs[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coordinate> newRegion = GetRegionTiles(x, y, width, height, map);
                    regions.Add(newRegion);

                    foreach (Coordinate tile in newRegion)
                    {
                        mapFalgs[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    private static List<Coordinate> GetRegionTiles(int startX, int startY, int width, int height, int[,] map)
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

            for (int x = tile.tileX - 1; x < tile.tileX + 2; x++)
            {
                for (int y = tile.tileY - 1; y < tile.tileY + 2; y++)
                {
                    if (AlgorithmUtils.IsInMapRange(x, y, width, height) && (y == tile.tileY || x == tile.tileX))
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