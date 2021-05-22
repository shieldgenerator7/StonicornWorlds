using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QueueTask
{
    public PlanetObjectType taskObject { get; private set; }
    public string taskObjectName;

    public enum Type
    {
        CONSTRUCT,
        CONVERT,
        DESTRUCT,
        PLANT
    }
    public Type type;
    public float StartCost =>
        (type == Type.CONVERT) ? taskObject.convertCost : taskObject.startCost;

    public Vector2 pos;

    [SerializeField]
    private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Min(value, taskObject.progressRequired);
            onProgressChanged?.Invoke(Percent);
        }
    }
    public delegate void OnProgressChanged(float percent);
    public event OnProgressChanged onProgressChanged;

    //Returns a number between 0 and 1: 0 = not started, 1 = completed
    public float Percent => progress / taskObject.progressRequired;

    public bool Started => progress > 0;

    public bool Completed => progress == taskObject.progressRequired;

    public QueueTask(PlanetObjectType taskObject, Vector2 pos, Type type)
    {
        this.taskObject = taskObject;
        this.taskObjectName = taskObject.typeName;
        this.pos = pos;
        this.type = type;
    }

    public void inflate()
    {
        this.taskObject = Managers.Constants.getType(this.taskObjectName);
    }

    public static implicit operator bool(QueueTask qt)
        => qt != null;
}
