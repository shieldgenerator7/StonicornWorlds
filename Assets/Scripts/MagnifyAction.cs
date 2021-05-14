using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnifyAction : ToolAction
{
    public override Color color => Color.white;

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Planet.Planet.hasPod(pos);

    public override void takeAction(List<Vector2> posList)
    {
        //If any of the positions are on the planet
        if (posList.Any(v => isActionValidAt(v)))
        {
            Vector2 center = posList.Aggregate((sum, v) => sum + v) / posList.Count;
            Managers.Camera.Locked = true;
            Managers.Camera.autoFrame(center, posList);
        }
        else
        {
            Managers.Camera.Locked = false;
            Managers.Camera.autoFrame(posList);
        }
    }
}
