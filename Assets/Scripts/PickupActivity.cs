using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupActivity : Activity
{
    public PickupActivity(Stonicorn stonicorn) : base(stonicorn) { }

    public override Stonicorn.Action action => Stonicorn.Action.PICKUP;

    public override bool canStart
        => stonicorn.rest > 0
        && stonicorn.toolbeltResources < stonicorn.toolbeltSize
        && Managers.Resources.anyCoreNonEmpty()
        && Managers.Queue.getAvailableTasks().Count > 0;

    public override bool canContinue
        => stonicorn.toolbeltResources < stonicorn.toolbeltSize
        && Managers.Resources.getResourcesAt(stonicorn.locationOfInterest) > 10;

    public override bool isDone
        => stonicorn.toolbeltResources == stonicorn.toolbeltSize;

    public override float ActivityRange => stonicorn.workRange;

    public override void doActivity()
    {
        float need = Mathf.Clamp(
            stonicorn.transferRate * Time.deltaTime,
            0,
            stonicorn.toolbeltSize - stonicorn.toolbeltResources
            );
        PodContent magma = Managers.Planet.Planet.getPod(stonicorn.locationOfInterest)
            .getContent(Managers.Constants.getPodContentType("MagmaContainer"));
        float give = Mathf.Clamp(need, 0, magma.Var);
        stonicorn.toolbeltResources += give;
        magma.Var -= give;
    }

    public override Vector2 chooseActivityLocation()
    {
        Vector2 corePos = Managers.Resources.getClosestNonEmptyCore(stonicorn.position);
        return corePos;
    }
}
