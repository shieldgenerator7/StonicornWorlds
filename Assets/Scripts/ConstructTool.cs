using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstructTool : PlanetObjectTypeTool
{
    public override void inputDown(Vector2 pos)
    {
        Vector2 clickedUI = Managers.Edge.edges.FirstOrDefault(
            edge => Managers.Planet.checkAddPodUI(pos, edge)
            );
        if (clickedUI != Vector2.zero
            && Managers.Planet.PlanetObjectType is PodType pt
            && Managers.Planet.canBuildAtPosition(
                pt,
                pos
                ))
        {
            Managers.Queue.addToQueue(
                new QueueTask(
                    pt,
                    clickedUI,
                    QueueTask.Type.CONSTRUCT
                    )
                );
        }
    }

    public override void inputMove(Vector2 pos)
    {
    }

    public override void inputUp(Vector2 pos)
    {
    }
}
