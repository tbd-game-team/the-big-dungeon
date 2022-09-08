using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// @author: Sunny Valley Studio 
/// available online at: https://github.com/SunnyValleyStudio/Unity_2D_Procedural_Dungoen_Tutorial
/// </summary>
public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // Up
        new Vector2Int(1,0), // Right
        new Vector2Int(0,-1), // Down
        new Vector2Int(-1,0) // Left
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1), // Up-Right
        new Vector2Int(1,-1), // Right-Down
        new Vector2Int(-1,-1), // Down-Left
        new Vector2Int(-1,1) // Left-Up
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // Up
        new Vector2Int(1,1), // Up-Right
        new Vector2Int(1,0), // Right
        new Vector2Int(1,-1), // Right-Down
        new Vector2Int(0,-1), // Down
        new Vector2Int(-1,-1), // Down-Left
        new Vector2Int(-1,0), // Left
        new Vector2Int(-1,1) // Left-Up
    };
}