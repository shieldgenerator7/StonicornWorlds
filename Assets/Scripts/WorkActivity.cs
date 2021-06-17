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
        && stonicorn.getTaskPriorities()
            .Any(task => Managers.Queue.isTaskAvailable(task));

    public override bool canContinue
        => stonicorn.rest > 0
        && stonicorn.toolbeltResources > 0
        && stonicorn.task;

    public override float ActivityRange => stonicorn.workRange;

    public override Stonicorn.Action action => Stonicorn.Action.WORK;

    public override Vector2 chooseActivityLocation()
    {
        if (stonicorn.task == null || stonicorn.task.Completed)
        {
            stonicorn.task = stonicorn.taskPriorities
                .First(task => Managers.Queue.isTaskAvailable(task));
        }
        return stonicorn.task.pos;
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
