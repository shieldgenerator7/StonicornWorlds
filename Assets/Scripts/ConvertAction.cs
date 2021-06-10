using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertAction : EditToolAction
{
    public override Color color => Managers.Input.PodType.uiColor;

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Queue.plans.hasPod(pos)
        && Managers.Planet.canPlanBuildAtPosition(Managers.Input.PodType, pos);

    protected override void takeAction(Vector2 pos)
    {
        Managers.Queue.addToQueue(
            new QueueTask(
                Managers.Input.PodType,
                pos,
                QueueTask.Type.CONVERT
                )
            );
    }
}
