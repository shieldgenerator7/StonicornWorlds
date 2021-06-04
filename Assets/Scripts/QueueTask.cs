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
        PLANT,
        DELIVER,
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
        private set
        {
            progress = Mathf.Min(value, taskObject.progressRequired);
            onProgressChanged?.Invoke(Percent);
        }
    }
    public delegate void OnProgressChanged(float percent);
    public event OnProgressChanged onProgressChanged;

    /// <summary>
    /// Uses the given resources to progress this task
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="resources"></param>
    /// <returns>amount of resources left after making progress</returns>
    public float makeProgress(float amount, float resources)
    {
        float startCost = StartCost;
        float required = startCost * amount / taskObject.progressRequired;
        float take = Mathf.Clamp(required, 0, resources);
        if (required == take)
        {
            Progress += amount;
        }
        else
        {
            float work = taskObject.progressRequired * take / startCost;
            Progress += work;
        }
        return resources - take;
    }

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
