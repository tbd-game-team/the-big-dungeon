using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public TilemapVisualizer tilemapVisualizer = null;
    public int minRoomWidth = 8;
    public int minRoomHeight = 8;
    public int dungeonWidth = 100;
    public int dungeonHeight = 100;

    public int cellularDensity = 45;
    public int cellularIterations = 10;

    public int wallRegionsThreshold = 5;
    public int roomRegionsThreshold = 5;

    void Awake()
    {
        CreateDungeon();
    }
    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <returns></returns>
    private void CreateDungeon()
    {
        List<BoundsInt> binarySpaces = SplitDungeonSpace();


        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        floor = CreateCellularAutomataRooms(binarySpaces);

        int[,] dungeonMap = AlgorithmUtils.HashSetToMap(floor, dungeonWidth, dungeonHeight);
        CellularAutomataAlgorithm.FilterWalls(wallRegionsThreshold, dungeonMap, dungeonWidth, dungeonHeight);
        List<BoundsInt> finalRooms = CellularAutomataAlgorithm.FilterRooms(roomRegionsThreshold, dungeonMap, dungeonWidth, dungeonHeight);
        HashSet<Vector2Int> corridors = CreateCorridors(floor, finalRooms);
        floor.UnionWith(corridors);

        corridors = CreateCorridors(floor, binarySpaces);
        floor.UnionWith(corridors);

        dungeonMap = AlgorithmUtils.HashSetToMap(floor, dungeonWidth, dungeonHeight);
        CellularAutomataAlgorithm.FilterWalls(20, dungeonMap, dungeonWidth, dungeonHeight);
        floor = AlgorithmUtils.MapToHashSet(new Vector2Int(0, 0), dungeonMap, dungeonWidth, dungeonHeight);

        ActorGenerator.PlaceActors( finalRooms, dungeonMap, dungeonWidth, dungeonHeight);

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }
    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <returns></returns>
    private List<BoundsInt> SplitDungeonSpace()
    {
        Vector3Int bottomLeftCorner = new Vector3Int(0, 0, 0);
        Vector3Int upperRightCorner = new Vector3Int(dungeonWidth, dungeonHeight, 0);
        BoundsInt dungeonSpace = new BoundsInt(bottomLeftCorner, upperRightCorner);
        List<BoundsInt> roomsList = BinarySpacePartitioningAlgorithm.BSP(dungeonSpace, minRoomWidth, minRoomHeight);
        return roomsList;
    }
    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="roomsList"></param>
    /// <returns></returns>
    private HashSet<Vector2Int> CreateCellularAutomataRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (BoundsInt room in roomsList)
        {
            int width = room.size.x;
            int height = room.size.y;
            Vector2Int startPosition = (Vector2Int)room.min;
            HashSet<Vector2Int> cellularRoom = CellularAutomataAlgorithm.CA(startPosition, width, height, cellularIterations, cellularDensity, wallRegionsThreshold, roomRegionsThreshold);
            floor.UnionWith(cellularRoom);
        }
        return floor;
    }
    
    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="roomsList"></param>
    /// <returns></returns> 
    private HashSet<Vector2Int> CreateCorridors(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        int[,] dungeonMap = AlgorithmUtils.HashSetToMap(floor, dungeonWidth, dungeonHeight);
        List<Vector2Int> roomCenters = AlgorithmUtils.CreateRoomCenters(roomsList, dungeonMap, dungeonWidth, dungeonHeight);
        HashSet<Vector2Int> corridors = CorridorGenerator.ConnectSpaces(roomCenters);
        return corridors;
    }
}




