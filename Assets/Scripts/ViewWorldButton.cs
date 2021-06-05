using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewWorldButton : ToolButton
{
    protected override void Start()
    {
        base.Start();
        canClickWhenActive = true;
    }

    public override void activate()
    {
        Managers.Camera.FocusObject = null;
    }

    protected override bool isActive()
        => Managers.Camera.FocusObject == null;
}
