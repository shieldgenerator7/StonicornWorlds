using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTask
{
    public ScriptableObject taskObject { get; private set; }

    public enum Type
    {
        CONSTRUCT,
        CONVERT,
        DESTRUCT,
        PLANT
    }
    public Type type { get; private set; }
    public float startCost => 100;

    public Vector2 pos { get; private set; }

    private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Min(value, progressRequired);
            onProgressChanged?.Invoke(progress);
        }
    }
    public delegate void OnProgressChanged(float progress);
    public event OnProgressChanged onProgressChanged;

    private float progressRequired;

    //Returns a number between 0 and 1: 0 = not started, 1 = completed
    public float Percent => progress / progressRequired;

    public bool Started => progress > 0;

    public bool Completed => progress == progressRequired;

    public QueueTask(ScriptableObject taskObject, Vector2 pos, Type type)
    {
        this.taskObject = taskObject;
        this.pos = pos;
        this.type = type;
        if (taskObject is PodType pt)
        {
            this.progressRequired = pt.progressRequired;
        }
        else if (taskObject is PodContentType pct)
        {
            this.progressRequired = pct.progressRequired;
        }
    }

    public static implicit operator bool(QueueTask qt)
        => qt != null;
}
