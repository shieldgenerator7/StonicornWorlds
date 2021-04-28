using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pod
{
    public Vector2 pos;
    public PodType podType;

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
    }
}
