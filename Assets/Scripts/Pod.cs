using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod
{
    public Vector2 pos;
    [System.NonSerialized]
    public PodType podType;
    public string podTypeName;

    public List<PodContent> podContents = new List<PodContent>();

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.podTypeName = podType.name;
    }

    public Pod Clone()
    {
        Pod clone = new Pod(this.pos, this.podType);
        clone.podContents = podContents.ConvertAll(content => content.Clone(clone));
        return clone;
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod";
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
