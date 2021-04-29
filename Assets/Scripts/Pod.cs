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

    public static bool operator ==(Pod p1, Pod p2)
        => p1.pos == p2.pos && p1.podType == p2.podType;
    public static bool operator !=(Pod p1, Pod p2)
        => p1.pos != p2.pos || p1.podType != p2.podType;
}
