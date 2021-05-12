using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolActionButton : ToolButton
{
    public ToolAction toolAction;

    public override void activate()
    {
        Managers.Input.toolAction = toolAction;
    }
}
