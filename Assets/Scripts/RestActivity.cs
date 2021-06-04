using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestActivity : Activity
{
    public RestActivity(Stonicorn stonicorn) : base(stonicorn) { }

    public override bool canStart
        => stonicorn.rest < stonicorn.maxRest * 0.1f
        && (stonicorn.toolbeltResources == 0 || !Managers.Resources.anyCoreNonFull());

    public override bool canContinue
        => stonicorn.rest < stonicorn.maxRest;

    public override bool isDone
        => stonicorn.rest == stonicorn.maxRest;

    public override float ActivityRange => 0;

    public override Stonicorn.Action action => Stonicorn.Action.REST;

    public override void doActivity()
    {
        float prevRest = stonicorn.Rest;
        stonicorn.Rest += stonicorn.restSpeed * Time.deltaTime;
        if (stonicorn.Rest == stonicorn.maxRest)
        {
            //if just now finished resting
            if (prevRest != stonicorn.maxRest)
            {
                if (stonicorn.isHomeHouse())
                {
                    stonicorn.restSpeed += 1;
                    stonicorn.maxRest += 10;
                    stonicorn.Rest = stonicorn.maxRest;
                }
            }
        }
    }

    public override Vector2 chooseActivityLocation()
    {
        stonicorn.task = null;
        return stonicorn.homePosition;
    }
}
