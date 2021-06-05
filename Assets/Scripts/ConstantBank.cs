using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantBank : Manager
{
    [Header("Buttons")]
    [SerializeField]
    private int desiredWidth = 1920;
    [SerializeField]
    private int desiredHeight = 1080;
    public float buttonCheckRadius = 50;
    public float buttonSpacing = 100;
    private float originalCheckRadius;
    private float originalSpacing;

    public override void setup()
    {
        originalCheckRadius = buttonCheckRadius;
        originalSpacing = buttonSpacing;
    }

    public void updateScreenConstants(int width, int height)
    {
        int dim;
        int desiredDim;
        if (width > height)
        {
            dim = width;
            desiredDim = desiredWidth;
        }
        else
        {
            dim = height;
            desiredDim = desiredHeight;
        }
        buttonCheckRadius = originalCheckRadius * dim / desiredDim;
        buttonSpacing = originalSpacing * dim / desiredDim;
    }

    [Header("PodTypes")]
    public PodType corePodType;
    public PodType spacePodType;

    public List<PodType> allPodTypes;
    public List<PodContentType> allPodContentTypes;
    public List<PodContentType> allSolidPodContentTypes;

    public PlanetObjectType getType(string typeName)
    {
        PlanetObjectType pot = getPodType(typeName);
        if (!pot)
        {
            pot = getPodContentType(typeName);
        }
        return pot;
    }
    public PodType getPodType(string typeName)
        => allPodTypes.Find(podType => podType.typeName == typeName);
    public PodContentType getPodContentType(string typeName)
        => allPodContentTypes.Find(pct => pct.typeName == typeName);
}
