using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolActionButton : ToolButton
{
    public ToolAction toolAction;

    public List<PlanetObjectType> compatibleObjectTypes;

    public override void activate()
    {
        Managers.Input.ToolAction = toolAction;
        if (compatibleObjectTypes.Count > 0 &&
            !compatibleObjectTypes.Contains(Managers.Input.PlanetObjectType))
        {
            if (compatibleObjectTypes.Contains(Managers.Input.PodType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodType;
            }
            else if (compatibleObjectTypes.Contains(Managers.Input.PodContentType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodContentType;
            }
            else
            {
                Managers.Input.PlanetObjectType = compatibleObjectTypes[0];
            }
        }
    }

    protected override bool isActive()
        => Managers.Input.ToolAction == toolAction
        && (compatibleObjectTypes.Count == 0
        || compatibleObjectTypes.Contains(Managers.Input.PlanetObjectType));
}
