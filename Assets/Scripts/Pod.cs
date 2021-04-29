using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pod
{
    public Vector2 pos;
    public PodType podType;
    public float progress;
    public const float PROGRESS_REQUIRED = 100;

    public static Pod Null => new Pod(Vector2.zero, null);

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.progress = 0;
    }

    public static implicit operator bool(Pod p)
        => p.podType != null;

    public static bool operator ==(Pod p1, Pod p2)
        => p1.pos == p2.pos && p1.podType == p2.podType;
    public static bool operator !=(Pod p1, Pod p2)
        => p1.pos != p2.pos || p1.podType != p2.podType;
}
