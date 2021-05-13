using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodTypeBank : MonoBehaviour
{
    public PodType corePodType;
    public PodType spacePodType;

    public List<PodType> allPodTypes;
    public List<PodContentType> allPodContentTypes;

    public PodType getPodType(string podTypeName)
        => allPodTypes.Find(podType => podType.typeName == podTypeName);
}
