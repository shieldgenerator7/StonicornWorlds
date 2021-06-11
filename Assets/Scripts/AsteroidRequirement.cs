using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRequirement : Requirement
{
    public override bool isRequirementMet()
    {
        return FindObjectOfType<AsteroidController>() != null;
    }
}
