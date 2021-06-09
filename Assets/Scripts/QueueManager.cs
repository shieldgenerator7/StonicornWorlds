using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : Manager
{
    public float defaultWorkRate = 20;

    public List<QueueTask> queue => Managers.Planet.Planet.tasks;

    public void addToQueue(QueueTask task)
    {
        if (queue.Contains(task))
        {
            Debug.LogError("Task already in queue");
            return;
        }
        queue.Add(task);
        Managers.Planet.updatePlans(task);
        callOnQueueChanged();
    }
    public void callOnQueueChanged()
    {
        onQueueChanged?.Invoke(queue);
    }
    public delegate void OnQueueChanged(List<QueueTask> queue);
    public event OnQueueChanged onQueueChanged;

    public bool isTaskAvailable(QueueTask task)
    {
        //Context
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
                return Managers.Planet.canBuildAtPosition(
                    (PodType)task.taskObject,
                    task.pos
                    );
            case QueueTask.Type.CONVERT:
                return Managers.Planet.Planet.hasPod(task.pos) &&
                    Managers.Planet.canBuildAtPosition(
                        (PodType)task.taskObject,
                        task.pos
                        );
            case QueueTask.Type.PLANT:
                return Managers.Planet.canPlantAtPosition(
                    (PodContentType)task.taskObject,
                    task.pos
                    );
            case QueueTask.Type.DESTRUCT:
                return true;
            default:
                Debug.LogError("Unknown task type!: " + task.type);
                return false;
        }
    }

    public QueueTask getClosestTask(Vector2 pos, Vector2 exclude)
        => queue.FindAll(task => isTaskAvailable(task))
            .OrderBy(task => Vector2.Distance(task.pos, pos)).ToList()
            .FindAll(task => task.pos != exclude)
            .FirstOrDefault();

    public List<QueueTask> getAvailableTasks()
        => queue.FindAll(task => isTaskAvailable(task));

    public bool hasEmptyTaskAt(Vector2 pos)
        => queue.Any(task => task.pos == pos && !task.Started);

    public void cancelEmptyTasksAt(Vector2 pos)
    {
        queue
            .FindAll(task => task.pos == pos && !task.Started)
            .ForEach(task =>
            {
                Managers.Planet.cancelPlans(task);
                queue.Remove(task);
            });
        callOnQueueChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="task"></param>
    /// <returns>amount of resources left after working on the task</returns>
    public float workOnTask(QueueTask task, float workRate, float resources)
    {
        if (task == null)
        {
            return resources;
        }
        if (task.Completed)
        {
            //don't work on a task that's already completed
            return resources;
        }
        resources = task.makeProgress(workRate * Time.deltaTime, resources);
        if (task.Completed)
        {
            taskCompleted(task);
        }
        return resources;
    }

    void taskCompleted(QueueTask task)
    {
        queue.Remove(task);
        callOnQueueChanged();
        onTaskCompleted?.Invoke(task);
    }
    public delegate void TaskEvent(QueueTask task);
    public event TaskEvent onTaskCompleted;

    public override void setup()
    {
        callOnQueueChanged();
    }
}
