using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineEditTool : SelectTool
{
    protected override List<Vector2> getSelectList(Vector2 start, Vector2 end)
        => Managers.Planet.Planet.getHexPosBetween(start, end);
}
