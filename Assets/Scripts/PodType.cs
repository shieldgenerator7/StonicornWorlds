using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PodType", menuName = "PodType", order = 0)]
public class PodType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public GameObject podPrefab;
    public float progressRequired = 100;
}
