using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetObjectType : ScriptableObject
{
    public string typeName = "PodTypeName";
    public Sprite preview;
    public Color uiColor = Color.white;
    public GameObject prefab;
    public bool constructible = true;
    public bool solid = true;
    public float startCost = 100;
    public float convertCost = 25;
    public float progressRequired = 100;

    public override string ToString()
        => typeName;
}
