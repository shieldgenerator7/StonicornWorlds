using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : Manager
{
    public float magmaCapPerCore = 700;
    public PodContentType magmaContentType;

    private List<PodContent> magmaContents;

    public float Resources
    {
        get => magmaContents.Sum(magma => magma.Var);
        set
        {
            float oldAmount = Resources;
            float newAmount = Mathf.Clamp(value, 0, magmaCap);
            if (oldAmount != newAmount)
            {
                if (newAmount == 0)
                {
                    magmaContents.ForEach(magma => magma.Var = 0);
                }
                else if (newAmount == magmaCap)
                {
                    magmaContents.ForEach(magma => magma.Var = magmaCapPerCore);
                }
                else if (newAmount < oldAmount)
                {
                    float diff = newAmount - oldAmount;
                    int i = magmaContents.Count - 1;
                    while (!Mathf.Approximately(diff, 0))
                    {
                        float oldVarVal = magmaContents[i].Var;
                        magmaContents[i].Var = Mathf.Clamp(
                            oldVarVal + diff,
                            0,
                            magmaCapPerCore
                            );
                        diff -= magmaContents[i].Var - oldVarVal;
                        i--;
                    }
                }
                else if (newAmount > oldAmount)
                {
                    float diff = newAmount - oldAmount;
                    int i = 0;
                    while (!Mathf.Approximately(diff, 0))
                    {
                        float oldVarVal = magmaContents[i].Var;
                        magmaContents[i].Var = Mathf.Clamp(
                            oldVarVal + diff,
                            0,
                            magmaCapPerCore
                            );
                        diff -= magmaContents[i].Var - oldVarVal;
                        i++;
                    }
                }
                onResourcesChanged?.Invoke(newAmount);
            }
        }
    }
    public delegate void OnResourcesChanged(float resources);
    public event OnResourcesChanged onResourcesChanged;

    private float magmaCap;
    public float MagmaCap => magmaCap;


    public override void setup()
    {
        if (Resources == 0)
        {
            Resources = MagmaCap;
        }
    }

    public void updateResourceCaps(Planet p)
    {
        magmaContents = p.Pods(Managers.Constants.corePodType)
            .ConvertAll(core => core.getContent(magmaContentType));
        magmaCap = magmaContents.Count * magmaCapPerCore;
    }

    public float getResourcesAt(Vector2 pos)
    {
        Pod pod = Managers.Planet.Planet.getPod(pos);
        if (!pod || pod.podType != Managers.Constants.corePodType)
        {
            return 0;
        }
        return pod.getContent(magmaContentType).Var;
    }

    public Vector2 getClosestCore(Vector2 pos)
        => Managers.Planet.Planet
            .getClosestPod(pos, Managers.Constants.corePodType).worldPos;
    public Vector2 getClosestNonEmptyCore(Vector2 pos)
    {
        List<PodContent> magmaList = Managers.Planet.Planet
            .Pods(Managers.Constants.corePodType)
            .ConvertAll(pod => pod.getContent(magmaContentType))
            .FindAll(magma => magma && magma.Var > 0);
        if (magmaList.Count > 0)
        {
            return magmaList
                .OrderBy(magma => Vector2.Distance(pos, magma.container.worldPos))
                .ToList()[0].container.worldPos;
        }
        else
        {
            return getClosestCore(pos);
        }
    }

    public bool anyCoreNonFull()
    {
        return Managers.Planet.Planet
            .Pods(Managers.Constants.corePodType)
            .ConvertAll(pod => pod.getContent(magmaContentType))
            .FindAll(magma => magma && magma.Var < magmaCapPerCore)
            .Count > 0;
    }
    public Vector2 getClosestNonFullCore(Vector2 pos)
    {
        List<PodContent> magmaList = Managers.Planet.Planet
            .Pods(Managers.Constants.corePodType)
            .ConvertAll(pod => pod.getContent(magmaContentType))
            .FindAll(magma => magma && magma.Var < magmaCapPerCore);
        if (magmaList.Count > 0)
        {
            return magmaList
                .OrderBy(magma => Vector2.Distance(pos, magma.container.worldPos))
                .ToList()[0].container.worldPos;
        }
        else
        {
            return getClosestCore(pos);
        }
    }
}
