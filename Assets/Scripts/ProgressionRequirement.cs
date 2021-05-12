using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionRequirement
{
    public PodType podType;
    public int count;
    public float resourceRequirement = 0;
    public PodTypeButton button;

    public bool checkProgression()
    {
        if (Managers.Planet.planet.Pods(podType).Count >= count
            && Managers.Planet.Resources >= resourceRequirement)
        {
            button.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
}
