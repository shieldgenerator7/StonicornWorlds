using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PodType", menuName = "PodType", order = 0)]
public class PodType : PlanetObjectType
{
    public List<PodType> allowedNeighbors;
    public List<PodType> requiredNeighbors;
    public List<PodType> constructFromTypes;//a list of pod types that can be converted to this type
    public List<PodContentType> includedContentTypes;//list of content to put inside it automatically when it gets created
    public bool requireConvert = false;//true: adding to an empty space is disallowed


    public bool areAllNeighborsAllowed(List<PodType> podTypes)
    {
        if (allowedNeighbors.Count == 0)
        {
            return podTypes.TrueForAll(podType =>
                podType.allowedNeighbors.Count == 0
                || podType.allowedNeighbors.Contains(this)
                || podType == Managers.Constants.spacePodType
            );
        }
        else
        {
            return podTypes.TrueForAll(podType =>
                allowedNeighbors.Contains(podType)
                || podType.allowedNeighbors.Contains(this)
                || podType == Managers.Constants.spacePodType
            );
        }
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

    public bool canConvertFrom(PodType podType)
    {
        if (podType)
        {
            return constructFromTypes.Contains(podType)
                || podType == Managers.Constants.spacePodType;
        }
        else
        {
            if (constructFromTypes.Count == 0)
            {
                return true;
            }
            else if (requireConvert)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
