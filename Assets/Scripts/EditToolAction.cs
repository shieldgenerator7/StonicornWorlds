using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditToolAction : ToolAction
{
    public sealed override void takeAction(List<Vector2> posList)
    {
        //special case: no positions
        if (posList.Count == 0)
        {
            return;
        }
        //special case: only 1 position
        else if (posList.Count == 1)
        {
            if (isActionValidAt(posList[0]))
            {
                takeAction(posList[0]);
            }
            return;
        }
        //normal case:
        GroupedList<bool, Vector2> posGroupList = new GroupedList<bool, Vector2>(
            v => isActionValidAt(v)
            );
        posGroupList.AddAll(posList);
        posGroupList.GetList(true)
            .ForEach(v => takeAction(v));
        List<Vector2> invalidPosList = posGroupList.GetList(false);
        int invalidCount = -1;
        while (invalidPosList.Count != invalidCount)
        {
            invalidCount = invalidPosList.Count;
            for (int i = invalidPosList.Count - 1; i >= 0; i--)
            {
                Vector2 pos = invalidPosList[i];
                if (isActionValidAt(pos))
                {
                    takeAction(pos);
                    invalidPosList.RemoveAt(i);
                }
            }
        }
    }

    protected abstract void takeAction(Vector2 pos);
}
