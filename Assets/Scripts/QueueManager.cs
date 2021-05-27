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
        //Resources
        if (!task.Started && Managers.Resources.Resources < task.StartCost)
        {
            return false;
        }
        //Context
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
            case QueueTask.Type.CONVERT:
                return Managers.Planet.canBuildAtPosition(
                    (PodType)task.taskObject,
                    task.pos,
                    false
                    );
            case QueueTask.Type.PLANT:
                return Managers.Planet.canPlantAtPosition(
                    (PodContentType)task.taskObject,
                    task.pos,
                    false
                    );
            case QueueTask.Type.DESTRUCT:
                return true;
            default:
                Debug.LogError("Unknown task type!: " + task.type);
                return false;
        }
    }

    public QueueTask getClosestTask(Vector2 pos)
        => queue.FindAll(task => isTaskAvailable(task))
            .OrderBy(task => Vector2.Distance(task.pos, pos))
            .ToList().FirstOrDefault();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="task"></param>
    /// <returns>true if completed, false if not completed or not started</returns>
    public bool workOnTask(QueueTask task, float workRate)
    {
        if (task == null)
        {
            return false;
        }
        if (!task.Started)
        {
            if (Managers.Resources.Resources >= task.StartCost)
            {
                Managers.Resources.Resources -= task.StartCost;
                task.Progress = 0.01f;
            }
            else
            {
                return false;
            }
        }
        task.Progress += workRate * Time.deltaTime;
        if (task.Completed)
        {
            taskCompleted(task);
            return true;
        }
        return false;
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
