using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ResourceRequirement : Requirement
{
    public float resourcesRequired = 0;

    public override bool isRequirementMet()
        => Managers.Resources.Resources >= resourcesRequired;
}
