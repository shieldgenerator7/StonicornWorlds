using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolToolButton : ToolButton
{
    public Tool tool;

    protected override void activateImpl()
    {
        Managers.Input.tool = tool;
        Managers.Input.checkAllButtons();
    }

    protected override bool isActiveImpl()
        => Managers.Input.tool == tool;
}
