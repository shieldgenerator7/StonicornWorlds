using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PodContentType", menuName = "PodContentType", order = 1)]
public class PodContentType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public Sprite preview;
    public Color uiColor = Color.white;
    public GameObject podContentPrefab;
    public float progressRequired = 100;
    public List<PodType> podImplantTypes;
    public List<PodType> requiredGround;
    public List<PodType> requiredNeighbors;
    public List<PodContentType> requiredContent;

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
            (curPod && curPod.podContents.Any(
                content => requiredContent.Contains(content.contentType)
                ));
    }
}
