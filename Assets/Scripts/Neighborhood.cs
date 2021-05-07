using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Neighborhood<T>
{
    public T ceilingLeft;
    public T ceiling;
    public T ceilingRight;
    public T groundLeft;
    public T ground;
    public T groundRight;
    public T[] neighbors;

    public Neighborhood(HexagonNeighborhood hexnh, HexagonGrid<T> grid)
    {
        ground = grid.get(hexnh.ground);
        groundLeft = grid.get(hexnh.groundLeft);
        groundRight = grid.get(hexnh.groundRight);
        ceiling = grid.get(hexnh.ceiling);
        ceilingLeft = grid.get(hexnh.ceilingLeft);
        ceilingRight = grid.get(hexnh.ceilingRight);
        neighbors = new T[]
        {
            ground,
            groundLeft,
            groundRight,
            ceiling,
            ceilingLeft,
            ceilingRight,
        };
    }

    public override string ToString()
    {
        return "ground: " + groundLeft + "-" + ground + "-" + groundRight + "; "
            + " ceiling: " + ceilingLeft + "-" + ceiling + "-" + ceilingRight + ";";
    }
}
