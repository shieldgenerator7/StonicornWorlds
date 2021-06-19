using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectTypeButton : ToolButton
{
    public PlanetObjectType planetObjectType;

    public override Color Color => planetObjectType.uiColor;

    protected override void activateImpl()
    {
        Managers.Input.PlanetObjectType = planetObjectType;
    }

    protected override bool isActiveImpl()
        => Managers.Input.PlanetObjectType == planetObjectType;
}
