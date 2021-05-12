using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructAction : ToolAction
{
    public override void takeAction(List<Vector2> posList)
    {
        if (Managers.Input.PlanetObjectType is PodType pt)
        {
            posList.FindAll(pos => Managers.Planet.canBuildAtPosition(
                pt,
                pos
                ))
                .ForEach(pos => takeAction(pos, pt));
        }
    }

    void takeAction(Vector2 pos, PodType pt)
    {
        Managers.Queue.addToQueue(
            new QueueTask(
                pt,
                pos,
                QueueTask.Type.CONSTRUCT
                )
             );
    }
}
