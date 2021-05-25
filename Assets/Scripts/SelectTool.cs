using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectTool : Tool
{
    private List<Vector2> selectList = new List<Vector2>();

    private Vector2 startPos;
    private Vector2 endPos;

    public override void inputDown(Vector2 pos)
    {
        startPos = pos;
        endPos = pos;
        updateSelectList();
    }

    public override void inputMove(Vector2 pos)
    {
        endPos = pos;
        updateSelectList();
    }

    public override void inputUp(Vector2 pos)
    {
        Managers.Input.ToolAction.takeAction(selectList);
        selectList.Clear();
    }

    private void updateSelectList()
    {
        selectList = getSelectList(startPos, endPos);
    }

    protected abstract List<Vector2> getSelectList(Vector2 start, Vector2 end);
}
