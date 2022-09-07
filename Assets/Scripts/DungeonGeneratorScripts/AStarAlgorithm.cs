
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    private static float moveStraightCost = 10.0f;
    private static float moveDiagonalCost = 20.0f;

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="startCoordinate"></param>
    /// <param name="targetCoordinate"></param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    public static List<Coordinate> AStar(Coordinate startCoordinate, Coordinate targetCoordinate, int[,] map, int width, int height)
    {
        List<Coordinate> path = new List<Coordinate>();
        List<Coordinate> openList = new List<Coordinate> { startCoordinate };
        List<Coordinate> closedList = new List<Coordinate>();
        Coordinate pathCoordinate = new Coordinate();

        startCoordinate.cameFromeCoordinate = null;
        startCoordinate.gCost = 0;
        startCoordinate.hCost = CalculateDistanceCost(startCoordinate, targetCoordinate);
        startCoordinate.CalculateFCost();


        while (openList.Count > 0)
        {
            Coordinate currentCoordinate = GetLowestFCostTile(openList);
            pathCoordinate = currentCoordinate;
            if (currentCoordinate.EqulasTo(targetCoordinate))
            {
                path = CalculatedPath(pathCoordinate);
                break;
            }

            openList.Remove(currentCoordinate);
            closedList.Add(currentCoordinate);

            List<Coordinate> neighbours = GetNeighbours(currentCoordinate.x, currentCoordinate.y, width, height);

            foreach (Coordinate neighbourCoordinate in neighbours)
            {
                if (neighbourCoordinate.IsInList(closedList))
                {
                    continue;
                }
                if (map[neighbourCoordinate.x, neighbourCoordinate.y] == AlgorithmUtils.wallTile)
                {
                    closedList.Add(neighbourCoordinate);
                    continue;
                }

                float tentativeGCost = currentCoordinate.gCost + CalculateDistanceCost(currentCoordinate, neighbourCoordinate);

                if (tentativeGCost < neighbourCoordinate.gCost)
                {
                    neighbourCoordinate.cameFromeCoordinate = currentCoordinate;
                    neighbourCoordinate.gCost = tentativeGCost;
                    neighbourCoordinate.hCost = CalculateDistanceCost(neighbourCoordinate, targetCoordinate);
                    neighbourCoordinate.CalculateFCost();

                    if (!neighbourCoordinate.IsInList(openList))
                    {
                        openList.Add(neighbourCoordinate);
                    }
                }
            }
        }
        return path;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>#
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns></returns>
    private static List<Coordinate> GetNeighbours(int gridX, int gridY, int width, int height)
    {
        List<Coordinate> neighbours = new List<Coordinate>();
        for (int neighbourX = gridX - 1; neighbourX < gridX + 2; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY < gridY + 2; neighbourY++)
            {
                if (AlgorithmUtils.IsInMapRange(neighbourX, neighbourY, width, height))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        neighbours.Add(new Coordinate(neighbourX, neighbourY));
                    }
                }
            }
        }
        return neighbours;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static float CalculateDistanceCost(Coordinate a, Coordinate b)
    {
        float xDistance = Mathf.Abs(a.centerX - b.centerX);
        float yDistance = Mathf.Abs(a.centerY - b.centerY);
        float remainingDistance = Mathf.Abs(xDistance - yDistance);
        float minDist = Mathf.Min(xDistance, yDistance);
        return moveDiagonalCost * minDist + moveStraightCost * remainingDistance;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="tileList"></param>
    /// <returns></returns>
    private static Coordinate GetLowestFCostTile(List<Coordinate> tileList)
    {
        Coordinate lowestFCostTile = tileList[0];
        for (int i = 1; i < tileList.Count; i++)
        {
            if (tileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tileList[i];
            }
        }
        return lowestFCostTile;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// 
    /// </summary>
    /// <param name="targetCoordinate"></param>
    /// <returns></returns>
    private static List<Coordinate> CalculatedPath(Coordinate targetCoordinate)
    {
        List<Coordinate> pathList = new List<Coordinate>();
        pathList.Add(targetCoordinate);
        Coordinate currentCoordinate = targetCoordinate;
        while (currentCoordinate.cameFromeCoordinate != null)
        {
            pathList.Add(currentCoordinate.cameFromeCoordinate);
            currentCoordinate = currentCoordinate.cameFromeCoordinate;
        }
        pathList.Reverse();
        return pathList;
    }
}