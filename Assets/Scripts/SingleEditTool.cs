using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleEditTool : Tool
{
    public override void inputDown(Vector2 pos)
    {
        Managers.Input.ToolAction.takeAction(new List<Vector2>() {
            Managers.Planet.planet.getHexPos(pos)
        });
    }

    public override void inputMove(Vector2 pos)
    {
    }

    public override void inputUp(Vector2 pos)
    {
    }
}
