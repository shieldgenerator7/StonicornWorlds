using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectTypeButton : ToolButton
{
    public PlanetObjectType planetObjectType;

    public List<ToolAction> compatibleToolActions;

    public override void activate()
    {
        Managers.Input.PlanetObjectType = planetObjectType;
        if (!compatibleToolActions.Contains(Managers.Input.ToolAction))
        {
            Managers.Input.ToolAction = compatibleToolActions[0];
        }
    }

    protected override bool isActive()
        => Managers.Input.PlanetObjectType == planetObjectType
        && compatibleToolActions.Contains(Managers.Input.ToolAction);
}
