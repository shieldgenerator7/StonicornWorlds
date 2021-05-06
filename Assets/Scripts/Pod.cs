using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod
{
    public Vector2 pos;
    public PodType podType;

    public List<PodContent> podContents = new List<PodContent>();

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod";
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
