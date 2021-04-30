using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PodType", menuName = "PodType", order = 0)]
public class PodType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public GameObject podPrefab;
    public float progressRequired = 100;
    public List<PodType> allowedNeighbors;

    public bool areAllNeighborsAllowed(List<PodType> podTypes)
    {
        if (allowedNeighbors.Count == 0)
        {
            return podTypes.TrueForAll(podType =>
                podType.allowedNeighbors.Count == 0
                || podType.allowedNeighbors.Contains(this)
            );
        }
        else
        {
            return podTypes.TrueForAll(podType =>
                allowedNeighbors.Contains(podType)
                || podType.allowedNeighbors.Contains(this)
            );
        }
    }
}
