using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAction : ToolAction
{
    public override Color color => Managers.Input.PodContentType.uiColor;

    public override void takeAction(List<Vector2> posList)
    {
        posList.FindAll(pos => isActionValidAt(pos))
            .ForEach(pos => takeAction(pos));
    }

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Planet.canPlantAtPosition(Managers.Input.PodContentType, pos);

    private void takeAction(Vector2 pos)
    {
        Managers.Queue.addToQueue(
            new QueueTask(
                Managers.Input.PodContentType,
                pos,
                QueueTask.Type.PLANT
                )
            );
    }
}
