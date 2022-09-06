using System.Collections.Generic;


public class Coordinate 
{
    public int tileX;
    public int tileY;
    public float centerX;
    public float centerY;

    public float gCost = float.MaxValue;
    public float hCost;
    public float fCost = float.MaxValue;

    public Coordinate cameFromeCoordinate = null;

    public Coordinate() { }
    public Coordinate(int x, int y)
    {
        tileX = x;
        tileY = y;
        centerX = x + 0.5f;
        centerY = y + 0.5f;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public bool IsInList(List<Coordinate> list)
    {
        foreach (Coordinate c in list)
        {
            if (c.tileX == tileX && c.tileY == tileY)
            {
                return true; 
            }
        }
        return false;
    }
    
    /*
    * @author: Neele Kemper
    * 
    */
    public bool EqulasTo(Coordinate c)
    {
        if(tileX == c.tileX && tileY == c.tileY)
        {
            return true;
        }
        return false;
    }
}

