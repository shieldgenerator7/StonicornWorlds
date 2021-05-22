using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Manager
{
    public float resourceCapPerCore = 700;

    float resources;
    public float Resources
    {
        get => resources;
        set
        {
            resources = Mathf.Clamp(value, 0, ResourceCap);
            onResourcesChanged?.Invoke(resources);
        }
    }
    public delegate void OnResourcesChanged(float resources);
    public event OnResourcesChanged onResourcesChanged;

    public float ResourceCap => Managers.Planet.CoreCount * resourceCapPerCore;


    public override void setup()
    {
        Resources = ResourceCap;
    }
}
