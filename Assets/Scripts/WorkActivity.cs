using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkActivity : Activity
{
    public WorkActivity(Stonicorn stonicorn) : base(stonicorn) { }

    public override bool isDone => stonicorn.task.Completed;

    public override bool canStart
        => !stonicorn.Sleepy
        && stonicorn.toolbeltResources > 0
        && getTaskPriorities().Count > 0;

    public override bool canContinue
        => stonicorn.rest > 0
        && stonicorn.toolbeltResources > 0;

    public override float ActivityRange => stonicorn.workRange;

    public override Stonicorn.Action action => Stonicorn.Action.WORK;

    public override Vector2 chooseActivityLocation()
    {
        if (stonicorn.task == null || stonicorn.task.Completed)
        {
            if (stonicorn.taskPriorities.Count > 0)
            {
                stonicorn.task = stonicorn.taskPriorities[0];
            }
        }
        return stonicorn.task.pos;
    }

    List<QueueTask> getTaskPriorities()
    {
        if (stonicorn.taskPriorities == null || stonicorn.taskPriorities.Count == 0)
        {
            List<QueueTask> tasks = Managers.Queue.getAvailableTasks();
            if (tasks.Any(task => task.type == stonicorn.favoriteJobType))
            {
                tasks.RemoveAll(task => task.type != stonicorn.favoriteJobType);
            }
            tasks = sortTasks(tasks, stonicorn.taskPriority2);
            tasks = sortTasks(tasks, stonicorn.taskPriority);
            stonicorn.taskPriorities = tasks;
        }
        return stonicorn.taskPriorities;
    }

    static List<Stonicorn.TaskPriority> descendList = new List<Stonicorn.TaskPriority>() {
        Stonicorn.TaskPriority.FAR,
        Stonicorn.TaskPriority.EXPENSIVE,
        Stonicorn.TaskPriority.SLOW,
        Stonicorn.TaskPriority.STARTED,
        Stonicorn.TaskPriority.LAST,
        Stonicorn.TaskPriority.GROUP,
    };
    private List<QueueTask> sortTasks(List<QueueTask> tasks, Stonicorn.TaskPriority taskPriority)
    {
        Func<QueueTask, float> sortFunction = task => 0;
        switch (taskPriority)
        {
            case Stonicorn.TaskPriority.CLOSE:
            case Stonicorn.TaskPriority.FAR:
                sortFunction = task => Vector2.Distance(task.pos, stonicorn.position);
                break;
            case Stonicorn.TaskPriority.CHEAP:
            case Stonicorn.TaskPriority.EXPENSIVE:
                sortFunction = task => task.StartCost;
                break;
            case Stonicorn.TaskPriority.FAST:
            case Stonicorn.TaskPriority.SLOW:
                sortFunction = task => task.taskObject.progressRequired;
                break;
            case Stonicorn.TaskPriority.EMPTY:
            case Stonicorn.TaskPriority.STARTED:
                sortFunction = task => task.Percent;
                break;
            case Stonicorn.TaskPriority.NEXT:
            case Stonicorn.TaskPriority.LAST:
                sortFunction = task => Managers.Queue.queue.IndexOf(task);
                break;
            case Stonicorn.TaskPriority.SOLO:
            case Stonicorn.TaskPriority.GROUP:
                sortFunction = task => Managers.Planet.Planet.residents
                  .FindAll(stncrn => stncrn.task == task)
                  .Count;
                break;
            default:
                Debug.LogError("Unknown TaskPriority!: " + taskPriority);
                break;
        }
        bool descend = descendList.Contains(taskPriority);
        if (descend)
        {
            return tasks.OrderByDescending(sortFunction).ToList();
        }
        else
        {
            return tasks.OrderBy(sortFunction).ToList();
        }
    }

    public override void doActivity(float timeDelta)
    {
        stonicorn.toolbeltResources = Managers.Queue.workOnTask(
            stonicorn.task,
            stonicorn.workRate,
            stonicorn.toolbeltResources,
            timeDelta
            );
        stonicorn.Rest -= stonicorn.workRate * timeDelta;
        if (stonicorn.task && stonicorn.task.Completed)
        {
            stonicorn.taskPriorities.Remove(stonicorn.task);
        }
    }
}
