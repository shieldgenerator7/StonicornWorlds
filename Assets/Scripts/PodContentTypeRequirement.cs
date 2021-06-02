using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodContentTypeRequirement : Requirement
{
    public PodContentType podContentType;
    public int count;

    public override bool isRequirementMet()
        => Managers.Planet.Planet.Pods(Managers.Constants.spacePodType)
            .FindAll(pod => pod.hasContent(podContentType)).Count
            >= count;
}
