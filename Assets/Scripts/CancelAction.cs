using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAction : EditToolAction
{
    public override Color color => Color.red;

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Queue.hasEmptyTaskAt(pos);

    protected override void takeAction(Vector2 pos)
    {
        Managers.Queue.cancelEmptyTasksAt(pos);
    }
}
