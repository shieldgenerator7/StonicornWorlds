using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverActivity : Activity
{
    readonly PickupActivity pickup;
    readonly DropoffActivity dropoff;

    bool droppingOff = false;

    Func<PodContent, bool> sourceFunc;
    Func<PodContent, bool> destinationFunc;
    Func<PodContent, bool> filterFunc;

    public DeliverActivity(Stonicorn stonicorn, PickupActivity pickup, DropoffActivity dropoff)
        : base(stonicorn)
    {
        this.pickup = pickup;
        this.dropoff = dropoff;

        filterFunc = (magma) =>
             Vector2.Distance(magma.container.worldPos, stonicorn.position) > ActivityRange + 0.1f;
        sourceFunc = (magma) =>
            magma.Var >= Managers.Resources.nearEmpty + stonicorn.toolbeltSize + 1
            && filterFunc(magma);
        destinationFunc = (magma) =>
            magma.Var < Managers.Resources.nearEmpty
            && filterFunc(magma);
    }

    public override Stonicorn.Action action
        => (droppingOff)
            ? Stonicorn.Action.DROPOFF
            : Stonicorn.Action.PICKUP;

    public override bool canStart
        => stonicorn.favoriteJobType == QueueTask.Type.DELIVER
        && !stonicorn.Sleepy
        && Managers.Resources.anyCore(sourceFunc)
        && Managers.Resources.anyCore(destinationFunc);

    public override bool canContinue
        => stonicorn.rest > 0 &&
        (droppingOff)
            ? dropoff.canContinue
            : pickup.canContinue;

    public override bool isDone
        => (droppingOff)
            ? dropoff.isDone
            : pickup.isDone;

    public override float ActivityRange => stonicorn.workRange;

    public override void doActivity(float timeDelta)
    {
        if (droppingOff)
        {
            dropoff.doActivity(timeDelta);
        }
        else
        {
            pickup.doActivity(timeDelta);
        }
        stonicorn.rest -= stonicorn.workRate * timeDelta / 2;
    }

    public override Vector2 chooseActivityLocation()
    {
        droppingOff = stonicorn.toolbeltResources > 0;
        if (droppingOff)
        {
            //dropoff
            return Managers.Resources.getClosestCore(stonicorn.position, destinationFunc);
        }
        else
        {
            //pickup
            return Managers.Resources.getClosestCore(stonicorn.position, sourceFunc);
        }
    }
}
