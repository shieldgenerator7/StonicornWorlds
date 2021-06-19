using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolActionButton : ToolButton
{
    public ToolAction toolAction;

    protected override void activateImpl()
    {
        Managers.Input.ToolAction = toolAction;
    }

    protected override bool isActiveImpl()
        => Managers.Input.ToolAction == toolAction;
}
