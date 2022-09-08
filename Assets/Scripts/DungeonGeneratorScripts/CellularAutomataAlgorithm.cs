using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public static class CellularAutomataAlgorithm
{
    /// <summary>
    /// @author: Neele Kemper
    /// Create a hash set representing the rooms in the dungeon using the cellular automaton (CA) .
    /// A CA is a collection of cells arranged in a grid of specified shape, such that each cell changes state as a function of time, according to a defined set of rules driven by the states of neighboring cell
    /// </summary>
    /// <param name="startPosition">position of the bottom left corner of the subspace (the dungon has been divided by BSP subspace)</param>
    /// <param name="spaceWidth">width of space</param>
    /// <param name="spaceHeight">height of space</param>d
    /// <param name="iterations">number of iteration for the application of cellular automata rules.</param>
    /// <param name="density">density of the floor cells</param>
    /// <returns></returns>
    public static HashSet<Vector2Int> CA(Vector2Int startPosition, int spaceWidth, int spaceHeight, int iterations, int density)
    {
        int[,] map = MakeNoiseGrid(density, spaceWidth, spaceHeight);
        for (int i = 0; i < iterations; i++)
        {
            ApplyCellularAutomaton(map, spaceWidth, spaceHeight);
        }
        HashSet<Vector2Int> room = AlgorithmUtils.MapToHashSet(startPosition, map, spaceWidth, spaceHeight);
        return room;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Create a random grid of floor (live) and wall (dead) cells.
    /// </summary>
    /// <param name="density">density of the floor cells</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>random grid</returns>
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
    /// Do a cellular automata iteration and apply the rules to all cells. 
    /// </summary>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static void ApplyCellularAutomaton(int[,] map, int width, int height)
    {
        // iterate over all cells in the dungeon
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallCount = AlgorithmUtils.CountSurroundingWalls(x, y, map, width, height);
                // if celle has less than 4 neighbors, it becomes a wall (dead)
                if (neighbourWallCount > 4)
                {
                    map[x, y] = AlgorithmUtils.wallTile;
                }
                // if celle has more than 4 neighbors, it becomes a floor (alive)
                else if (neighbourWallCount < 4)
                {
                    map[x, y] = AlgorithmUtils.floorTile;
                }

            }
        }
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Remove all wall regions that are smaller than the passed threshold from the dungeon.
    /// </summary>
    /// <param name="wallRegionsThreshold">minimum size for wall regions</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static void FilterWalls(int wallRegionsThreshold, int[,] map, int width, int height)
    {
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
    /// Remove all rooms (floor regions) that are smaller than the passed threshold from the dungeon.
    /// </summary>
    /// <param name="roomRegionsThreshold">minimum size for rooms</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>list of rooms larger than the threshold</returns>
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
    /// Get all regions of one type of the dungeon
    /// A region is an area of type a surrounded by coordinates of type b.
    /// </summary>
    /// <param name="coordType">type of the regions, i.e. floor or wall</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>list of regions</returns>
    private static List<List<Coordinate>> GetRegions(int coordType, int[,] map, int width, int height)
    {
        List<List<Coordinate>> regions = new List<List<Coordinate>>();
        int[,] mapFalgs = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFalgs[x, y] == 0 && map[x, y] == coordType)
                {
                    List<Coordinate> newRegion = GetRegionCoordinates(x, y, map, width, height);
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
    /// Get all coordinates of a region
    /// </summary>
    /// <param name="startX">x-coordinate</param>
    /// <param name="startY">y-coordinate</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>list of coordinates describing a region</returns>
    private static List<Coordinate> GetRegionCoordinates(int startX, int startY, int[,] map, int width, int height)
    {
        List<Coordinate> coordinates = new List<Coordinate>();
        // create a map where the coordinates are marked (value is set to 1) when they have been visited
        int[,] visitedCoordinates = new int[width, height];
        visitedCoordinates[startX, startY] = 1;

        // save the coordinate type (Floor or Wall) of the first coordinate
        int coordType = map[startX, startY];
        visitedCoordinates[startX, startY] = 1;

        // initialize the list of open coordinates and put the starting coordinate on the open queue
        Queue<Coordinate> openQueue = new Queue<Coordinate>();
        openQueue.Enqueue(new Coordinate(startX, startY));

        // while the open queue is not empty
        while (openQueue.Count > 0)
        {
            // remove first coordinate from queue
            Coordinate coord = openQueue.Dequeue();
            // push coord on the coordinates list
            coordinates.Add(coord);

            // iterate over all neighbor of the coordinate
            for (int x = coord.x - 1; x < coord.x + 2; x++)
            {
                for (int y = coord.y - 1; y < coord.y + 2; y++)
                {
                    if (AlgorithmUtils.IsInMapRange(x, y, width, height))
                    {
                        // if neighbor has not been visited yet and vpm same type is the start coordinate, add the neighbor to the open queue.
                        if (visitedCoordinates[x, y] == 0 && map[x, y] == coordType)
                        {
                            // mark the coordinate as visited.
                            visitedCoordinates[x, y] = 1;
                            openQueue.Enqueue(new Coordinate(x, y));
                        }
                    }
                }
            }
        }
        return coordinates;
    }

}