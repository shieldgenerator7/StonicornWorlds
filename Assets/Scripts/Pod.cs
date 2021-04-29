using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod
{
    public Vector2 pos;
    public PodType podType;
    public float progress;
    public const float PROGRESS_REQUIRED = 100;

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.progress = 0;
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
