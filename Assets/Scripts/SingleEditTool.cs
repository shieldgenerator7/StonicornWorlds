using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleEditTool : SelectTool
{
    protected override List<Vector2> getSelectList(Vector2 start, Vector2 end)
        => new List<Vector2>() { Managers.Planet.Planet.getHexPos(end) };
}
