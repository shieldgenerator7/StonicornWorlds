using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SelectTool : Tool
{
    private Vector2 startPos;
    private Vector2 endPos;

    private Vector2 prevIdlePosition;

    public sealed override void inputDown(Vector2 pos)
    {
        startPos = pos;
        endPos = pos;
        Managers.Input.SelectList = getSelectList(startPos, endPos);
    }

    public sealed override void inputMove(Vector2 pos)
    {
        endPos = pos;
        Managers.Input.SelectList = getSelectList(startPos, endPos);
    }

    public sealed override void inputUp(Vector2 pos)
    {
        Managers.Input.ToolAction.takeAction(Managers.Input.SelectList);
        Managers.Input.SelectList = new List<Vector2>();
    }

    public sealed override void inputIdle(Vector2 pos)
    {
        pos = Managers.Planet.Planet.getHexPos(pos);
        if (pos != prevIdlePosition)
        {
            prevIdlePosition = pos;
            Managers.Input.SelectList = getIdleSelectList(pos);
        }
    }

    protected abstract List<Vector2> getSelectList(Vector2 start, Vector2 end);
    protected virtual List<Vector2> getIdleSelectList(Vector2 pos)
        => new List<Vector2>() { pos };
}
