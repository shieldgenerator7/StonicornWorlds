using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverActivity : Activity
{
    readonly PickupActivity pickup;
    readonly DropoffActivity dropoff;

    Func<PodContent, bool> sourceFunc =
        (magma) => magma.Var >= Managers.Resources.halfFull;
    Func<PodContent, bool> destinationFunc =
        (magma) => magma.Var < Managers.Resources.nearEmpty;

    public DeliverActivity(Stonicorn stonicorn, PickupActivity pickup, DropoffActivity dropoff)
        : base(stonicorn)
    {
        this.pickup = pickup;
        this.dropoff = dropoff;
    }

    public override Stonicorn.Action action
        => (stonicorn.toolbeltResources > 0)
            ? Stonicorn.Action.DROPOFF
            : Stonicorn.Action.PICKUP;

    public override bool canStart
        => stonicorn.favoriteJobType == QueueTask.Type.DELIVER
        && stonicorn.rest > 0
        && Managers.Resources.anyCore(sourceFunc)
        && Managers.Resources.anyCore(destinationFunc);

        => (stonicorn.toolbeltResources > 0)
    public override bool canContinue
        => stonicorn.rest > 0 &&
            ? dropoff.canContinue
            : pickup.canContinue;

    public override bool isDone
        => (stonicorn.toolbeltResources > 0)
            ? dropoff.isDone
            : pickup.isDone;

    public override float ActivityRange => stonicorn.workRange;

    public override void doActivity()
    {
        if(stonicorn.toolbeltResources > 0)
        {
            dropoff.doActivity();
        }
        else
        {
            pickup.doActivity();
        }
        stonicorn.rest -= stonicorn.workRate * Time.deltaTime / 2;
    }

    public override Vector2 chooseActivityLocation()
    {
        if (stonicorn.toolbeltResources > 0)
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
