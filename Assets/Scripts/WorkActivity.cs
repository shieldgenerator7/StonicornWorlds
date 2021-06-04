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
        => stonicorn.rest > 0
        && stonicorn.toolbeltResources > 0
        && Managers.Queue.getAvailableTasks().Count > 0;

    public override bool canContinue
        => stonicorn.rest > 0
        && stonicorn.toolbeltResources > 0;

    public override float ActivityRange => stonicorn.workRange;

    public override Stonicorn.Action action => Stonicorn.Action.WORK;

    protected override Vector2 chooseActivityLocation()
    {
        if (stonicorn.task == null || stonicorn.task.Completed)
        {
            List<QueueTask> tasks = Managers.Queue.getAvailableTasks();
            if (tasks.Any(task=>task.type == stonicorn.favoriteJobType))
            {
                tasks.RemoveAll(task => task.type != stonicorn.favoriteJobType);
            }
            tasks = sortTasks(tasks, stonicorn.taskPriority2);
            tasks = sortTasks(tasks, stonicorn.taskPriority);
            stonicorn.task = tasks[0];
        }
        return stonicorn.task.pos;
    }
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
        bool descend = new List<Stonicorn.TaskPriority>() {
            Stonicorn.TaskPriority.FAR,
            Stonicorn.TaskPriority.EXPENSIVE,
            Stonicorn.TaskPriority.STARTED,
            Stonicorn.TaskPriority.LAST,
            Stonicorn.TaskPriority.GROUP,
        }.Contains(taskPriority);
        if (descend)
        {
            return tasks.OrderByDescending(sortFunction).ToList();
        }
        else
        {
            return tasks.OrderBy(sortFunction).ToList();
        }
    }

    public override void doActivity()
    {
        stonicorn.toolbeltResources = Managers.Queue.workOnTask(
            stonicorn.task,
            stonicorn.workRate,
            stonicorn.toolbeltResources
            );
        stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
    }
}
