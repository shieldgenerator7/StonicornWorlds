using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetObject
{
    [System.NonSerialized]
    public PlanetObjectType objectType;
    public string typeName;

    public PlanetObject(PlanetObjectType objectType)
    {
        this.objectType = objectType;
        this.typeName = objectType.typeName;
    }

    public static implicit operator bool(PlanetObject po)
        => po != null;
}
