
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{   
    private static float move_straight_cost = 10.0f;
    private static float move_diagonal_cost = 20.0f;

        
    /*
    * @author: Neele Kemper
    * 
    */
    public static List<Coordinate> AStar(Coordinate startCoordinate, Coordinate targetCoordinate, int[,]map, int width, int height)
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

            List<Coordinate> neighbours = GetNeighbours(currentCoordinate.tileX, currentCoordinate.tileY, width, height);

            foreach (Coordinate neighbourCoordinate in neighbours)
            {
                if(neighbourCoordinate.IsInList(closedList))
                {
                    continue;
                }
                if(map[neighbourCoordinate.tileX, neighbourCoordinate.tileY] == AlgorithmUtils.wallTile){
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

        
    /*
    * @author: Neele Kemper
    * 
    */
    private static List<Coordinate> GetNeighbours(int gridX, int gridY,  int width, int height)
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

    
    /*
    * @author: Neele Kemper
    * 
    */
    private static float CalculateDistanceCost(Coordinate a, Coordinate b)
    {
        float xDistance = Mathf.Abs(a.centerX - b.centerX);
        float yDistance = Mathf.Abs(a.centerY - b.centerY);
        float remainingDistance = Mathf.Abs(xDistance - yDistance);
        float minDist = Mathf.Min(xDistance, yDistance);
        return move_diagonal_cost * minDist + move_straight_cost * remainingDistance;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
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
    
    /*
    * @author: Neele Kemper
    * 
    */
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