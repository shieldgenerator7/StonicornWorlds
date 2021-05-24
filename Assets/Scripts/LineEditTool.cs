using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineEditTool : Tool
{
    private Vector2 origPos;
    private Vector2 endPos;

    public override void inputDown(Vector2 pos)
    {
        origPos = pos;
        endPos = pos;
    }

    public override void inputMove(Vector2 pos)
    {
        endPos = pos;
    }

    public override void inputUp(Vector2 pos)
    {
        List<Vector2> line = Managers.Planet.Planet.getHexPosBetween(origPos, endPos);
        Managers.Input.ToolAction.takeAction(
            line
            );
    }
}
