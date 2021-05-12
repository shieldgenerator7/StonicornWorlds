using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetObjectTypeTool : Tool
{
    public PlanetObjectType planetObjectType;

    public override void activate()
    {
        Managers.Input.PlanetObjectType = planetObjectType;
    }
}
