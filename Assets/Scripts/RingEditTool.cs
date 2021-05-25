using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEditTool : Tool
{
    private Vector2 endPos;

    public override void inputDown(Vector2 pos)
    {
        endPos = pos;
    }

    public override void inputMove(Vector2 pos)
    {
        endPos = pos;
    }

    public override void inputUp(Vector2 pos)
    {
        List<Vector2> ring = Managers.Planet.Planet.getHexPosRing(endPos);
        Managers.Input.ToolAction.takeAction(
            ring
            );
    }
}
