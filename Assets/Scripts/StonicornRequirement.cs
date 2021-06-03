using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornRequirement : Requirement
{
    public int count;

    public override bool isRequirementMet()
    {
        return Managers.Planet.Planet.residents.Count >= count;
    }
}
