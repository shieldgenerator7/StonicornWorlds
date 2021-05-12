using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetObjectType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public Sprite preview;
    public Color uiColor = Color.white;
    public GameObject prefab;
    public float progressRequired = 100;
}
