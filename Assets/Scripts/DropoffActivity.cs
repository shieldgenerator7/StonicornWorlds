using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropoffActivity : Activity
{
    public DropoffActivity(Stonicorn stonicorn) : base(stonicorn) { }

    public override Stonicorn.Action action => Stonicorn.Action.DROPOFF;

    public override bool canStart
        => stonicorn.toolbeltResources > 0
        && Managers.Resources.anyCoreNonFull();

    public override bool canContinue
        => stonicorn.toolbeltResources > 0
        && Managers.Resources.getResourcesAt(stonicorn.locationOfInterest)
            < Managers.Resources.magmaCapPerCore;

    public override bool isDone
        => stonicorn.toolbeltResources == 0;

    public override float ActivityRange => stonicorn.workRange;

    public override void doActivity()
    {
        float excess = Mathf.Clamp(
            stonicorn.transferRate * Time.deltaTime,
            0,
            stonicorn.toolbeltResources - 0
            );
        PodContent magma = Managers.Planet.Planet.getPod(stonicorn.locationOfInterest)
            .getContent(Managers.Constants.getPodContentType("MagmaContainer"));
        float take = Mathf.Clamp(excess, 0, Managers.Resources.magmaCapPerCore - magma.Var);
        stonicorn.toolbeltResources -= take;
        magma.Var += take;
    }

    protected override Vector2 chooseActivityLocation()
    {
        Vector2 corePos = Managers.Resources.getClosestNonFullCore(stonicorn.position);
        return corePos;
    }
}