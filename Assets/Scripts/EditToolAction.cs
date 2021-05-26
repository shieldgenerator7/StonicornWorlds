using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditToolAction : ToolAction
{
    public sealed override void takeAction(List<Vector2> posList)
    {
        foreach (Vector2 pos in posList)
        {
            if (isActionValidAt(pos))
            {
                takeAction(pos);
            }
        }
    }

    protected abstract void takeAction(Vector2 pos);
}
