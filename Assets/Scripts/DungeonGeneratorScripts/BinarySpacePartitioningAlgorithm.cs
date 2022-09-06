using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BinarySpacePartitioningAlgorithm
{
   
    /// <summary>
    /// @author: Neele Kemper
    /// Implements the Binary Space Partitioning algorithm.
    /// The space is arbitrarily divided into two subspaces. The subspaces are further subdivided recursively.
    /// </summary>
    /// <param name="dungeonSpace">space to be divided</param>
    /// <param name="minWidth">minimum width of the subspaces</param>
    /// <param name="minHeight">minimum height of the subspaces</param>
    /// <returns>list of subspaces</returns>
    public static List<BoundsInt> BSP(BoundsInt dungeonSpace, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(dungeonSpace);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            bool heightStatus = room.size.y >= 2 * minHeight;
            bool widthStatus = room.size.x >= 2 * minWidth;
            if (heightStatus && widthStatus)
            {
                if (Random.value < 0.5f)
                {
                    // split vertically
                    SplitSpace(0, roomsQueue, room);
                }
                else
                {
                    // split horizontally
                    SplitSpace(1, roomsQueue, room);
                }
            }
            else if (widthStatus)
            {
                // split vertically
                SplitSpace(0, roomsQueue, room);
            }
            else if (heightStatus)
            {
                // split horizontally
                SplitSpace(1,roomsQueue, room);

            }
            else
            {
                roomsList.Add(room);
            }
        }
        return roomsList;
    }
    
    /// <summary>
    /// @author: Neele Kemper
    /// Divides space horizontally or vertically into two subspaces
    /// </summary>
    /// <param name="orientation">specifies whether space is split horizontally (1) or vertically (0).</param>
    /// <param name="roomsQueue">queue of spaces</param>
    /// <param name="room">space to split</param>
    /// <returns></returns>
    private static void SplitSpace(int orientation, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        Vector3Int bottomLeftCorner1 = new Vector3Int();
        Vector3Int size1 = new Vector3Int();
        Vector3Int bottomLeftCorner2 = new Vector3Int();
        Vector3Int size2 = new Vector3Int();

        int width = room.size.x;
        int height = room.size.y;
        int depth = room.size.z;

        if (orientation == 0)
        {
            // split vertically
            var xSplit = Random.Range(1, width);
            bottomLeftCorner1 = room.min;
            size1 = new Vector3Int(xSplit, height, depth);

            bottomLeftCorner2 = new Vector3Int(room.xMin + xSplit, room.yMin, room.zMin);
            size2 = new Vector3Int(width - xSplit, height, depth);
        }
        else
        {
            //split horizontally
            var ySplit = Random.Range(1, height);
            bottomLeftCorner1 = room.min;
            size1 = new Vector3Int(width, ySplit, depth);

            bottomLeftCorner2 = new Vector3Int(room.xMin, room.yMin + ySplit, room.zMin);
            size2 = new Vector3Int(width, height - ySplit, depth);
        }
        BoundsInt room1 = new BoundsInt(bottomLeftCorner1, size1);
        BoundsInt room2 = new BoundsInt(bottomLeftCorner2, size2);
        // append new subspaces to queue
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}