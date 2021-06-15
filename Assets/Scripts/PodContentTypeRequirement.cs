using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodContentTypeRequirement : Requirement
{
    public PodContentType podContentType;
    public int count;

    public override bool isRequirementMet()
        => (podContentType != null)
        ? Managers.Planet.Planet.Pods(podContentType).Count >= count
        : Managers.Planet.Planet.Pods(Managers.Constants.spacePodType)
            .Count(pod => pod.hasContentConstructible()) >= count;
}
