using System.Collections.Generic;
using UnityEngine;


public class Coordinate
{
    public int x;
    public int y;

    // variables required for the A* algorithm.
    public float centerX;
    public float centerY;
    public float gCost = float.MaxValue;
    public float hCost;
    public float fCost = float.MaxValue;
    public Coordinate cameFromeCoordinate = null;

    public Coordinate() { }
    public Coordinate(int newX, int newY)
    {
        x = newX;
        y = newY;
        centerX = x + 0.5f;
        centerY = y + 0.5f;
    }

    public Coordinate(Vector3 worldCoordinate)
    {
        TilemapVisualizer tilemapVisualizer = Object.FindObjectOfType<TilemapVisualizer>();
        var cell = tilemapVisualizer.floorTilemap.WorldToCell(worldCoordinate);
        x = cell.x;
        y = cell.y;
        centerX = x + 0.5f;
        centerY = y + 0.5f;
    }

    public Vector3 ToPosition()
    {
        TilemapVisualizer tilemapVisualizer = Object.FindObjectOfType<TilemapVisualizer>();
        return tilemapVisualizer.floorTilemap.CellToWorld(new Vector3Int(x, y, 0));
    }

    public Vector3 ToCentralPosition()
    {
        return ToPosition() + new Vector3(0.5f, 0.5f, 0);
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Calculates the FCost of the coordinate for the A* algorithm.
    /// </summary>
    /// <returns></returns>
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Checks if the coordinate is in the passed list of coordinates.
    /// </summary>
    /// <param name="list">list of coordinate</param>
    /// <returns>true, if coordinate is in list, otherwise false.</returns>
    public bool IsInList(List<Coordinate> list)
    {
        foreach (Coordinate c in list)
        {
            if (c.x == x && c.y == y)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Checks if the coordinate equals the passed coordinate.
    /// </summary>
    /// <param name="c">other coordinate</param>
    /// <returns>true, if coordinates are equal, otherwise false.</returns>
    public bool EqulasTo(Coordinate c)
    {
        if (x == c.x && y == c.y)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return "[" + x + "/" + y + "]";
    }
}
