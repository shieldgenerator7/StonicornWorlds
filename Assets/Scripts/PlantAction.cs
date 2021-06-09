using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAction : EditToolAction
{
    public override Color color => Managers.Input.PodContentType.uiColor;

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Planet.canPlanPlantAtPosition(Managers.Input.PodContentType, pos);

    protected override void takeAction(Vector2 pos)
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
