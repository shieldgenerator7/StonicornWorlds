using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodTypeRequirement : Requirement
{
    public PodType podType;
    public int count;
    [Tooltip("Does it require that the max amount of the given podtype is 0?")]
    public bool requireNone = false;

    public override bool isRequirementMet()
        => (podType != null)
            ? (requireNone) 
                ? Managers.Planet.Planet.Pods(podType).Count == 0 
                : Managers.Planet.Planet.Pods(podType).Count >= count
            : Managers.Planet.Planet.PodsNotEmpty.Count >= count;
}
