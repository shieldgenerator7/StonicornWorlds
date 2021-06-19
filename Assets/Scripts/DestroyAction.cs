using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAction : EditToolAction
{
    public override Color color => Color.red;

    public override bool isActionValidAt(Vector2 pos)
    {
        Pod pod = Managers.Planet.Planet.getPod(pos);
        if (!pod)
        {
            return false;
        }
        if (pod.podType == Managers.Constants.spacePodType)
        {
            //if (pod.hasContentSolid())
            //{
            //    return true;
            //}
            //else
            //{
                return false;
            //}
        }
        return true && Managers.Queue.plans.hasPod(pos); ;
    }

    protected override void takeAction(Vector2 pos)
    {
        Managers.Queue.addToQueue(
            new QueueTask(
                Managers.Planet.Planet.getPod(pos).podType,
                pos,
                QueueTask.Type.DESTRUCT
                )
            );
    }
}
