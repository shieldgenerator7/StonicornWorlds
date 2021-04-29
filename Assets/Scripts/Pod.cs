using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod
{
    public const float PROGRESS_REQUIRED = 100;

    public Vector2 pos;
    public PodType podType;
    private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Min(value, PROGRESS_REQUIRED);
            onProgressChanged?.Invoke(progress);
        }
    }

    public delegate void OnProgressChanged(float progress);
    public event OnProgressChanged onProgressChanged;

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.progress = 0;
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
