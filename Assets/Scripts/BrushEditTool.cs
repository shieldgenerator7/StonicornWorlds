using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrushEditTool : SelectTool
{
    private Vector2 startPos;
    private List<Vector2> posList = new List<Vector2>();

    protected override List<Vector2> getSelectList(Vector2 start, Vector2 end)
    {
        Vector2 startHex = Managers.Planet.Planet.getHexPos(start);
        Vector2 endHex = Managers.Planet.Planet.getHexPos(end);
        if (this.startPos != startHex)
        {
            Debug.Log("start: " + startPos + ", new start: " + startHex);
            this.startPos = startHex;
            posList.Clear();
            posList.Add(startHex);
        }
        if (!posList.Contains(endHex))
        {
            posList.Add(endHex);
        }
        return posList.ToList();
    }
}
