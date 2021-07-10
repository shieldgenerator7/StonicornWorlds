using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SaveFileRequirement : Requirement
{
    public override bool isRequirementMet()
        => Managers.File.FileExists;
}
