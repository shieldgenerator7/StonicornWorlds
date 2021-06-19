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

    protected override void activateImpl()
    {
        Managers.QueueEffects.setShowEffects(!isActive());
    }

    protected override bool isActiveImpl()
    {
        return Managers.QueueEffects.showEffects;
    }
}
