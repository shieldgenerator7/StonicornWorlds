using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompatibilitySet
{
    public bool ganzEgal = false;
    public List<Tool> tools;
    public List<ToolAction> toolActions;
    public List<PlanetObjectType> planetObjectTypes;

    public bool compatibleWithCurrentInput()
    {
        return tools.Contains(Managers.Input.tool)
            && toolActions.Contains(Managers.Input.ToolAction)
            && planetObjectTypes.Contains(Managers.Input.PlanetObjectType);
    }

    public void setInputCompatible()
    {
        if (!tools.Contains(Managers.Input.tool))
        {
            Managers.Input.tool = (tools.Count > 0) ? tools[0] : null;
        }
        if (!toolActions.Contains(Managers.Input.ToolAction))
        {
            Managers.Input.ToolAction = (toolActions.Count > 0) ? toolActions[0] : null;
        }
        if (!planetObjectTypes.Contains(Managers.Input.PlanetObjectType))
        {
            if (planetObjectTypes.Contains(Managers.Input.PodType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodType;
            }
            else if (planetObjectTypes.Contains(Managers.Input.PodContentType))
            {
                Managers.Input.PlanetObjectType = Managers.Input.PodContentType;
            }
            else
            {
                Managers.Input.PlanetObjectType = (planetObjectTypes.Count > 0) ? planetObjectTypes[0] : null;
            }
        }
    }
}
