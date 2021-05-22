using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionRequirement
{
    public PodType podType;
    public int count;
    public float resourceRequirement = 0;
    public bool requireSaveFile = false;
    public ToolButton button;

    public bool checkProgression()
    {
        bool podTypeCheck = (podType)
            ? Managers.Planet.Planet.Pods(podType).Count >= count
            : Managers.Planet.Planet.PodsAll.Count >= count;
        if (podTypeCheck
            && Managers.Resources.Resources >= resourceRequirement
            && (!requireSaveFile || ES3.FileExists(Managers.File.fileName)))
        {
            button.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
}
