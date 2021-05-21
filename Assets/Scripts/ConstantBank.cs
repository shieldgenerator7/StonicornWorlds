using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantBank : MonoBehaviour
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

    public void setup()
    {
        originalCheckRadius = buttonCheckRadius;
        originalSpacing = buttonSpacing;
    }

    public void updateScreenConstants(int width, int height)
    {
        buttonCheckRadius = originalCheckRadius;
        buttonSpacing = originalSpacing * width / desiredWidth;
    }

    [Header("PodTypes")]
    public PodType corePodType;
    public PodType spacePodType;

    public List<PodType> allPodTypes;
    public List<PodContentType> allPodContentTypes;

    public PodType getPodType(string podTypeName)
        => allPodTypes.Find(podType => podType.typeName == podTypeName);
}
