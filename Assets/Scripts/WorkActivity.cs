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
        //&& stonicorn.toolbeltResources > 0
        && Managers.Queue.getAvailableTasks().Count > 0;

    public override bool canContinue
        => stonicorn.rest > 0;
    //&& stonicorn.toolbeltResources > 0;

    public override float ActivityRange => stonicorn.workRange;

    public override Stonicorn.Action action => Stonicorn.Action.WORK;

    protected override Vector2 chooseActivityLocation()
    {
        if (stonicorn.task == null)
        {
            stonicorn.task = Managers.Queue.getAvailableTasks()
                .OrderBy(task => Vector2.Distance(task.pos, stonicorn.position))
                .ToList()[0];
        }
        return stonicorn.task.pos;
    }

    public override void doActivity()
    {
        stonicorn.toolbeltResources = Managers.Queue.workOnTask(
            stonicorn.task,
            stonicorn.workRate,
            100//stonicorn.toolbeltResources
            );
        stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
    }
}
