using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectTypeButton : ToolButton
{
    public PlanetObjectType planetObjectType;

    public override void activate()
    {
        Managers.Input.PlanetObjectType = planetObjectType;
    }
}
