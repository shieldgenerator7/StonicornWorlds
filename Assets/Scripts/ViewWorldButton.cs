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

    protected override void activateImpl()
    {
        if (!isActiveImpl())
        {
            Managers.Camera.FocusObject = null;
        }
    }

    protected override bool isActiveImpl()
        => Managers.Camera.FocusObject == null;
}
