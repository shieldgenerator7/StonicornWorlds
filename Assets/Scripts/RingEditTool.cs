using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEditTool : SelectTool
{
    protected override List<Vector2> getSelectList(Vector2 start, Vector2 end)
        => Managers.Planet.Planet.getHexPosRing(start, end);
}
