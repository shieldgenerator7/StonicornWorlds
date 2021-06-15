using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRaftersButton : ToolButton
{
    protected override void Start()
    {
        base.Start();
        canClickWhenActive = true;
    }

    public override void activate()
    {
        Managers.QueueEffects.setShowEffects(!isActive());
    }

    protected override bool isActive()
    {
        return Managers.QueueEffects.showEffects;
    }
}
