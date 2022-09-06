using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public static class ActorGenerator
{   
    private static Vector3 playerPosition = new Vector3();
    private static Vector3 targetPosition = new Vector3();
    private static List<Vector3> enemyPositions = new List<Vector3>();

    
    /*
    * @author: Neele Kemper
    * 
    */
    public static void PlaceActors(HashSet<Vector2Int> floor, List<BoundsInt> rooms, int[,] map, int width, int height)
    {
        rooms.Sort((r1, r2) => ((r1.size.x * r1.size.y).CompareTo(r2.size.x * r2.size.y)));
        List<BoundsInt> enemyRooms = new List<BoundsInt>(rooms);

        List<Vector2Int> roomCenters = AlgorithmUtils.CreateRoomCenters(rooms, width, height, map);

        Vector2Int startPosition = roomCenters[0];
        playerPosition = new Vector3(startPosition.x, startPosition.y, 0);
        Coordinate startCoordinate = new Coordinate(startPosition.x, startPosition.y);

        roomCenters.Remove(startPosition);
        enemyRooms.Remove(rooms[0]);

        List<int> pathLengthList = new List<int>();

        foreach (Vector2Int center in roomCenters)
        {
            Coordinate roomCoordinate = new Coordinate(center.x, center.y);
            List<Coordinate> path = AStarAlgorithm.AStar(startCoordinate, roomCoordinate, map, width, height);
            pathLengthList.Add(path.Count);
        }

        int maxDistance = pathLengthList.Max();
        int targetIndex = pathLengthList.IndexOf(maxDistance);
        targetPosition = new Vector3(roomCenters[targetIndex].x, roomCenters[targetIndex].y, 0);

        enemyRooms.RemoveAt(targetIndex);
        pathLengthList.RemoveAt(targetIndex);
        CalculateEnemyPositions(enemyRooms, pathLengthList, map, width, height);

    }
        
    /*
    * @author: Neele Kemper
    * 
    */
    private static void CalculateEnemyPositions(List<BoundsInt> rooms, List<int> pathLengths, int[,] map, int width, int height)
    {
        int nSteps = 3;
        int stepsWide = (pathLengths.Max() - pathLengths.Min()) / nSteps;

        for (int i = 0; i < rooms.Count; i++)
        {
            BoundsInt room = rooms[i];
            int length = pathLengths[i];
            float enemyDensity = 0.0f;
            if (length <= stepsWide)
            {
                enemyDensity = 0.03f;
            }
            else if (length <= 2 * stepsWide)
            {
                enemyDensity = 0.06f;
            }
            else
            {
                enemyDensity = 0.09f;
            }
            int roomArea = (room.size.x * room.size.y)/2;
            int nEnemies = Mathf.Max(1, (int)(roomArea * enemyDensity));
            if (nEnemies == 0)
            {

            }
            int count = 0;
            while (count < nEnemies)
            {
                int x = Random.Range(room.min.x, room.max.x);
                int y = Random.Range(room.min.y, room.max.y);
                if (AlgorithmUtils.IsInMapRange(x, y, width, height))
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    int surroundingWalls = CellularAutomataAlgorithm.CountSurroundingWalls(x,y,width,height,map);
                    bool isFloor =(map[x, y] == AlgorithmUtils.floorTile);
                    if (isFloor && !enemyPositions.Contains(pos) && surroundingWalls < 2)  
                    {   
                        enemyPositions.Add(pos);
                        count++;

                    }
                }

            }
        }
    }

        
    /*
    * @author: Neele Kemper
    * 
    */
    public static Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }

    
    /*
    * @author: Neele Kemper
    * 
    */
    public static Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public static List<Vector3> GetEnemyPositions()
    {
        return enemyPositions;
    }
}

