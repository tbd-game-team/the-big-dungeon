using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Visualization")]
    [SerializeField]
    private TilemapVisualizer tilemapVisualizer = null;


    [Header("Dungeon Generation")]
    [SerializeField]
    private int minRoomWidth = 8;
    [SerializeField]
    private int minRoomHeight = 8;
    [SerializeField]
    private int dungeonWidth = 100;
    [SerializeField]
    private int dungeonHeight = 100;
    [SerializeField]
    private int cellularAutomatonDensity = 45;
    [SerializeField]
    private int cellularAutomatonIterations = 10;
    [SerializeField]
    private int wallRegionsThreshold = 5;
    [SerializeField]
    private int roomRegionsThreshold = 5;

    void Awake()
    {
        CreateDungeon();
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Generate the dungeon
    /// </summary>
    /// <returns></returns>
    private void CreateDungeon()
    {
        HashSet<Vector2Int> dungeon = new HashSet<Vector2Int>();

        // 1. Partition the space into subspaces using the Binary Space Partitioning (BSP) algorithm.
        List<BoundsInt> binarySpaces = SplitDungeonSpace();

        // 2. Create rooms in each subspace using the Cellular Automaton (CA) Algorithm. 
        dungeon = CreateCellularAutomatonRooms(binarySpaces);
        int[,] dungeonMap = AlgorithmUtils.HashSetToMap(dungeon, dungeonWidth, dungeonHeight);

        // 3. Remove too small space and wall regions.
        CellularAutomataAlgorithm.FilterWalls(wallRegionsThreshold, dungeonMap, dungeonWidth, dungeonHeight);
        List<BoundsInt> finalRooms = CellularAutomataAlgorithm.FilterRooms(roomRegionsThreshold, dungeonMap, dungeonWidth, dungeonHeight);

        // 4. Connect the closest CA rooms with each other.
        HashSet<Vector2Int> corridors = CreateCorridors(dungeon, finalRooms);
        dungeon.UnionWith(corridors);

        // 5. Connect the closest subspaces with each other.
        corridors = CreateCorridors(dungeon, binarySpaces);
        dungeon.UnionWith(corridors);
        dungeonMap = AlgorithmUtils.HashSetToMap(dungeon, dungeonWidth, dungeonHeight);

        // 6. Remove the too small wall regions that were created during the creation of the coridor.
        CellularAutomataAlgorithm.FilterWalls(20, dungeonMap, dungeonWidth, dungeonHeight);
        dungeon = AlgorithmUtils.MapToHashSet(new Vector2Int(0, 0), dungeonMap, dungeonWidth, dungeonHeight);

        // 7. Calculate the position of the player, the target coin and the enemies.
        SpawnPositionGenerator.CalculatePositions(finalRooms, dungeonMap, dungeonWidth, dungeonHeight);

        // 8. Visualize the dungeon
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(dungeon);
        WallGenerator.CreateWalls(dungeon, tilemapVisualizer);
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Divide the space into subspaces using the BSP algorithm.
    /// </summary>
    /// <returns>list of subspaces</returns>
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
    /// Create rooms in the dungeon subspaces using CA.
    /// </summary>
    /// <param name="roomsList">list of dungeon subspaces</param>
    /// <returns>hash set of rooms created by CA</returns>
    private HashSet<Vector2Int> CreateCellularAutomatonRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (BoundsInt room in roomsList)
        {
            int width = room.size.x;
            int height = room.size.y;
            Vector2Int startPosition = (Vector2Int)room.min;
            HashSet<Vector2Int> cellularRoom = CellularAutomataAlgorithm.CA(startPosition, width, height, cellularAutomatonIterations, cellularAutomatonDensity);
            floor.UnionWith(cellularRoom);
        }
        return floor;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Connect the rooms closest to each other with coridors.
    /// </summary>
    /// <param name="floor">hash set that defines the current dungeon.</param>
    /// <param name="roomsList">list of rooms</param>
    /// <returns>hash set of corridors</returns> 
    private HashSet<Vector2Int> CreateCorridors(HashSet<Vector2Int> dungeon, List<BoundsInt> roomsList)
    {
        int[,] dungeonMap = AlgorithmUtils.HashSetToMap(dungeon, dungeonWidth, dungeonHeight);
        List<Vector2Int> roomCenters = AlgorithmUtils.CreateRoomCenters(roomsList, dungeonMap, dungeonWidth, dungeonHeight);
        HashSet<Vector2Int> corridors = CorridorGenerator.ConnectSpaces(roomCenters);
        return corridors;
    }
}




