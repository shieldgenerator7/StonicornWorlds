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
    public float startCost { get; private set; }

    public Vector2 pos { get; private set; }

    private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Min(value, progressRequired);
            onProgressChanged?.Invoke(Percent);
        }
    }
    public delegate void OnProgressChanged(float percent);
    public event OnProgressChanged onProgressChanged;

    private float progressRequired;

    //Returns a number between 0 and 1: 0 = not started, 1 = completed
    public float Percent => progress / progressRequired;

    public bool Started => progress > 0;

    public bool Completed => progress == progressRequired;

    public QueueTask(PlanetObjectType taskObject, Vector2 pos, Type type)
    {
        this.taskObject = taskObject;
        this.pos = pos;
        this.type = type;
        this.progressRequired = taskObject.progressRequired;
        this.startCost = taskObject.startCost;
        if (this.type == Type.CONVERT)
        {
            this.startCost = taskObject.convertCost;
        }
    }

    public static implicit operator bool(QueueTask qt)
        => qt != null;
}
