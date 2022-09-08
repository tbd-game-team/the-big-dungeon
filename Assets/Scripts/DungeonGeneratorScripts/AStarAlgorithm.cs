
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    // cost for a straight movement
    private static float moveStraightCost = 10.0f;
    // cost for a diagonal movement
    private static float moveDiagonalCost = 15.0f;

    /// <summary>
    /// @author: Neele Kemper
    /// Calculates the shortest path from the passed start coordinate to the passed target coordinate using the A* search algorithm.
    /// </summary>
    /// <param name="startCoordinate">start point</param>
    /// <param name="targetCoordinate">target point</param>
    /// <param name="map">dungeon map</param>
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>shortest path</returns>
    public static List<Coordinate> AStar(Coordinate startCoordinate, Coordinate targetCoordinate, int[,] map, int width, int height)
    {

        List<Coordinate> path = new List<Coordinate>();
        // initialize the open list and put the starting node on the open list 
        List<Coordinate> openList = new List<Coordinate> { startCoordinate };
        // initialize the closed list
        List<Coordinate> closedList = new List<Coordinate>();
        Coordinate pathCoordinate = new Coordinate();

        // initialize the starting node values
        startCoordinate.cameFromeCoordinate = null;
        startCoordinate.gCost = 0;
        startCoordinate.hCost = CalculateDistanceCost(startCoordinate, targetCoordinate);
        startCoordinate.CalculateFCost();

        // while the open list is not empty
        while (openList.Count > 0)
        {
            // find the node with the least f on the open list
            Coordinate currentCoordinate = GetLowestFCostCoordinate(openList);
            pathCoordinate = currentCoordinate;
            // if currentCoordinate is the goal, stop search
            if (currentCoordinate.EqulasTo(targetCoordinate))
            {
                path = CalculatedPath(pathCoordinate);
                break;
            }

            // pop currentCoordinate of the open list
            openList.Remove(currentCoordinate);
            // push currentCoordinate on the closed list
            closedList.Add(currentCoordinate);

            // generate currentCoordinate's 8 successors and set their parents to currentCoordinate
            List<Coordinate> neighbours = GetNeighbours(currentCoordinate.x, currentCoordinate.y, width, height);

            // for each successor
            foreach (Coordinate neighbourCoordinate in neighbours)
            {
                // if successor is in closed list, skip this successor 
                if (neighbourCoordinate.IsInList(closedList))
                {
                    continue;
                }
                // if successor is blocked, skip this successor 
                if (map[neighbourCoordinate.x, neighbourCoordinate.y] == AlgorithmUtils.wallTile)
                {
                    closedList.Add(neighbourCoordinate);
                    continue;
                }

                float newGCost = currentCoordinate.gCost + CalculateDistanceCost(currentCoordinate, neighbourCoordinate);

                // if the tentative g cost is less than the g cost of the successor, compute g, h and f for successor
                if (newGCost < neighbourCoordinate.gCost)
                {
                    neighbourCoordinate.cameFromeCoordinate = currentCoordinate;
                    neighbourCoordinate.gCost = newGCost;
                    neighbourCoordinate.hCost = CalculateDistanceCost(neighbourCoordinate, targetCoordinate);
                    neighbourCoordinate.CalculateFCost();

                    // if the successor is not already in the open list, add it.
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
    /// Return the eight neighbour coordinates of the passed coordinate.
    /// </summary>
    /// <param name="gridX">x-coordinate</param>
    /// <param name="gridY">y-coordinate</param>#
    /// <param name="width">width of the dungeon</param>
    /// <param name="height">height of the dungeon</param>
    /// <returns>list of neighbours</returns>
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
    /// Calculate the distance cost between two passed coordinates.
    /// </summary>
    /// <param name="a">first coordinate </param>
    /// <param name="b">second coordinate </param>
    /// <returns>distance cost</returns>
    private static float CalculateDistanceCost(Coordinate a, Coordinate b)
    {
        float xDistance = Mathf.Abs(a.centerX - b.centerX);
        float yDistance = Mathf.Abs(a.centerY - b.centerY);
        float remainingDistance = Mathf.Abs(xDistance - yDistance);
        float minDist = Mathf.Min(xDistance, yDistance);
        float cost = moveDiagonalCost * minDist + moveStraightCost * remainingDistance;
        return cost;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Get the coordinate with the lowest fCost from the passed list. 
    /// </summary>
    /// <param name="list">list of coordinates</param>
    /// <returns>coordinate with the lowest fCost </returns>
    private static Coordinate GetLowestFCostCoordinate(List<Coordinate> list)
    {
        Coordinate lowestFCostCoord = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < lowestFCostCoord.fCost)
            {
                lowestFCostCoord = list[i];
            }
        }
        return lowestFCostCoord;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Calculate the path that leads to the passed target coordinate.
    /// </summary>
    /// <param name="targetCoordinate">target coordinate</param>
    /// <returns>path to the destination </returns>
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