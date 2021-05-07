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

    public void inflate()
    {
        this.podType = Resources.Load<PodType>("PodTypes/" + podTypeName);
        podContents.ForEach(pc => pc.inflate(this));
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod";
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
