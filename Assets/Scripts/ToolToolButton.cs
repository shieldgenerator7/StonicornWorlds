using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolToolButton : ToolButton
{
    public Tool tool;

    public override void activate()
    {
        Managers.Input.tool = tool;
        Managers.Input.checkAllButtons();
    }

    protected override bool isActive()
        => Managers.Input.tool == tool;
}
