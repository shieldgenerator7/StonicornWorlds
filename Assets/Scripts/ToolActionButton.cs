using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolActionButton : ToolButton
{
    public ToolAction toolAction;

    public List<PlanetObjectType> compatibleObjectTypes;

    public override void activate()
    {
        if (compatibleObjectTypes.Count > 0 &&
            !compatibleObjectTypes.Contains(Managers.Input.PlanetObjectType))
        {
            if (Managers.Input.PodType &&
                compatibleObjectTypes.Contains(Managers.Input.PodType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodType;
            }
            else if (Managers.Input.PodContentType &&
                compatibleObjectTypes.Contains(Managers.Input.PodContentType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodContentType;
            }
            else
            {
                Managers.Input.PlanetObjectType = compatibleObjectTypes[0];
            }
        }
        Managers.Input.ToolAction = toolAction;
    }

    protected override bool isActive()
        => Managers.Input.ToolAction == toolAction
        && (compatibleObjectTypes.Count == 0
        || compatibleObjectTypes.Contains(Managers.Input.PlanetObjectType));
}
