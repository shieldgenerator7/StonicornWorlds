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
}
