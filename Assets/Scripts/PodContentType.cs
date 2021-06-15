using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PodContentType", menuName = "PodContentType", order = 1)]
public class PodContentType : PlanetObjectType
{
    public float initialVarValue = 0;
    public List<PodType> podImplantTypes;
    public List<PodType> requiredGround;
    public List<PodType> requiredNeighbors;
    public List<PodContentType> requiredContent;
    public List<PodContentType> requiredPlanContent;

    public bool hasRequiredGround(PodType podType)
    {
        return requiredGround.Count == 0 ||
            (podType && requiredGround.Contains(podType));
    }

    public bool canPlantIn(PodType podType)
    {
        return podType &&
            (podImplantTypes.Count == 0 || podImplantTypes.Contains(podType));
    }

    public bool isRequiredNeighborPresent(List<PodType> podTypes)
    {
        if (requiredNeighbors.Count == 0)
        {
            return podTypes.Count > 0;
        }
        else
        {
            return podTypes.Any(podType => requiredNeighbors.Contains(podType));
        }
    }

    public bool hasRequiredContent(Pod curPod)
    {
        return requiredContent.Count == 0 ||
            (curPod && requiredContent.Any(
                ctntType => curPod.hasContent(ctntType)
                ));
    }
    public bool hasRequiredPlanContent(Pod curPod)
    {
        return requiredPlanContent.Count == 0 ||
            (curPod && requiredPlanContent.Any(
                ctntType => curPod.hasContent(ctntType)
                ));
    }
    public bool hasRoomFor(Pod curPod)
    {
        return !curPod || (
            !curPod.hasContent(this) &&
            !(this.solid && curPod.hasContentSolid())
            );
    }
}
