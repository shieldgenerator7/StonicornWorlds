using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Neighborhood
{
    public Pod ceilingLeft;
    public Pod ceiling;
    public Pod ceilingRight;
    public Pod groundLeft;
    public Pod ground;
    public Pod groundRight;
    public Pod[] neighbors;

    public Neighborhood(HexagonNeighborhood hexnh, HexagonGrid grid)
    {
        ground = grid.get(hexnh.ground);
        groundLeft = grid.get(hexnh.groundLeft);
        groundRight = grid.get(hexnh.groundRight);
        ceiling = grid.get(hexnh.ceiling);
        ceilingLeft = grid.get(hexnh.ceilingLeft);
        ceilingRight = grid.get(hexnh.ceilingRight);
        neighbors = new Pod[]
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
