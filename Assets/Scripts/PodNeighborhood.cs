using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PodNeighborhood
{
    public Pod ceilingLeft;
    public Pod ceiling;
    public Pod ceilingRight;
    public Pod groundLeft;
    public Pod ground;
    public Pod groundRight;

    public override string ToString()
    {
        return "ground: " + groundLeft + "-" + ground + "-" + groundRight + "; "
            + " ceiling: " + ceilingLeft + "-" + ceiling + "-" + ceilingRight + ";";
    }
}
