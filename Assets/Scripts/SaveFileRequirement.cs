using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFileRequirement : Requirement
{
    public override bool isRequirementMet()
        => ES3.FileExists(Managers.File.fileName);
}
