using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructAction : ToolAction
{
    public override Color color => Managers.Input.PodType.uiColor;

    public override void takeAction(List<Vector2> posList)
    {
        posList.FindAll(pos => isActionValidAt(pos))
            .ForEach(pos => takeAction(pos));
    }

    public override bool isActionValidAt(Vector2 pos)
        => !Managers.Planet.FuturePlanet.hasPod(pos)
        && Managers.Planet.canBuildAtPosition(Managers.Input.PodType, pos);

    void takeAction(Vector2 pos)
    {
        Managers.Queue.addToQueue(
            new QueueTask(
                Managers.Input.PodType,
                pos,
                QueueTask.Type.CONSTRUCT
                )
             );
    }
}
