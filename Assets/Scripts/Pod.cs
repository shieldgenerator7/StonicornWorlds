using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod
{
    public Vector2 pos;
    public PodType podType;

    public List<PodContent> podContents = new List<PodContent>();

    private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Min(value, podType.progressRequired);
            onProgressChanged?.Invoke(progress);
        }
    }
    public delegate void OnProgressChanged(float progress);
    public event OnProgressChanged onProgressChanged;

    public bool Started => progress > 0;

    public bool Completed
    {
        get => progress == podType.progressRequired;
        set
        {
            Progress = (value) ? podType.progressRequired : 0;
        }
    }

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.progress = 0;
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod"
            + ((!Completed) ? "[" + Progress + "/" + podType.progressRequired + "]" : "");
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
