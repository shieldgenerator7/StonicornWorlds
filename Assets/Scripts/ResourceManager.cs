using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : Manager
{
    public float magmaCapPerCore = 700;
    public float nearEmpty = 100;
    public float halfFull = 401;
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
                    while (!Mathf.Approximately(diff, 0) && i >= 0)
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
                    while (!Mathf.Approximately(diff, 0) && i < magmaContents.Count)
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
        PodContent magma = magmaContents.FirstOrDefault(
            magma => magma.container.worldPos == pos
            );
        if (magma)
        {
            return magma.Var;
        }
        return 0;
    }

    public Vector2 getClosestCore(Vector2 pos)
        => Managers.Planet.Planet
            .getClosestPod(pos, Managers.Constants.corePodType).worldPos;

    public bool anyCoreNonEmpty()
        => magmaContents.Any(magma => magma.Var > nearEmpty);

    public Vector2 getClosestNonEmptyCore(Vector2 pos)
    {
        List<PodContent> magmaList = magmaContents
            .FindAll(magma => magma.Var > 0);
        if (magmaList.Count > 0)
        {
            magmaList = magmaList
                .OrderByDescending(magma => magma.Var)
                .OrderBy(magma => Vector2.Distance(pos, magma.container.worldPos))
                .ToList();
            PodContent magma = magmaList.FirstOrDefault(magma => magma.Var > nearEmpty);
            if (!magma)
            {
                magma = magmaList[0];
            }
            return magma.container.worldPos;
        }
        else
        {
            return getClosestCore(pos);
        }
    }

    public bool anyCoreNonFull()
        => magmaContents.Any(magma => magma.Var < magmaCapPerCore);
    public Vector2 getClosestNonFullCore(Vector2 pos)
    {
        List<PodContent> magmaList = magmaContents
            .FindAll(magma => magma.Var < magmaCapPerCore);
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

    public bool anyCore(Func<PodContent, bool> findFunc)
        => magmaContents.Any(magma => findFunc(magma));

    public Vector2 getClosestCore(Vector2 pos, Func<PodContent, bool> findFunc)
    {
        List<PodContent> magmaList = magmaContents
            .FindAll(magma => findFunc(magma));
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
