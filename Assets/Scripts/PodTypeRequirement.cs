using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodTypeRequirement : Requirement
{
    public PodType podType;
    public int count;

    public override bool isRequirementMet()
        => (podType != null)
            ? Managers.Planet.Planet.Pods(podType).Count >= count
            : Managers.Planet.Planet.PodsNotEmpty.Count >= count;
}
