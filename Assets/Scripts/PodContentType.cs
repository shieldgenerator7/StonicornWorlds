using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PodContentType", menuName = "PodContentType", order = 1)]
public class PodContentType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public Sprite preview;
    public Color uiColor = Color.white;
    public GameObject podContentPrefab;
    public List<PodType> podImplantTypes;
    public List<PodType> requiredGround;

    public bool hasRequiredGround(PodType podType)
    {
        return podType &&
            (requiredGround.Count == 0 || requiredGround.Contains(podType));
    }

    public bool canPlantIn(PodType podType)
    {
        return podType &&
            (podImplantTypes.Count == 0 || podImplantTypes.Contains(podType));
    }
}
