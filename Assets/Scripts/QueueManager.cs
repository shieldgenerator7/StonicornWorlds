using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : Manager
{
    public float defaultWorkRate = 20;

    public List<QueueTask> queue => Managers.Planet.Planet.tasks;

    List<Stonicorn> workers => Managers.Planet.Planet.residents;

    public void addToQueue(QueueTask task)
    {
        if (queue.Contains(task))
        {
            Debug.LogError("Task already in queue");
            return;
        }
        workers.ForEach(w => w.queueTaskIndex = -1);
        queue.Add(task);
        callOnQueueChanged();
    }
    public void callOnQueueChanged()
    {
        onQueueChanged?.Invoke(queue);
    }
    public delegate void OnQueueChanged(List<QueueTask> queue);
    public event OnQueueChanged onQueueChanged;

    // Update is called once per frame
    void Update()
    {
        if (queue.Count > 0)
        {
            while (AnyWorkersFree)
            {
                //Try to find task not being worked on
                QueueTask task = queue.FirstOrDefault(
                    task => !workers.Any(
                        w => w.queueTaskIndex >= 0 && queue[w.queueTaskIndex] == task
                        )) ?? queue[0];
                //Try to start task
                if (!task.Started
                    && Managers.Resources.Resources >= task.StartCost)
                {
                    Managers.Resources.Resources -= task.StartCost;
                    task.Progress = 0.01f;
                }
                //Try to find a task that is already in progress
                if (!task.Started)
                {
                    task = queue.FirstOrDefault(p => p.Started) ?? queue[0];
                }
                //If task has been started,
                if (task.Started)
                {
                    //Work on it more
                    dispatchWorker(task);
                }
                //Else stop, can't do anything
                else
                {
                    break;
                }
            }
            workers.FindAll(w => w.queueTaskIndex >= 0)
                .ForEach(
                w =>
                {
                    QueueTask currentTask = queue[w.queueTaskIndex];
                    currentTask.Progress += w.workRate * Time.deltaTime;
                    if (currentTask.Completed)
                    {
                        taskCompleted(currentTask);
                        w.queueTaskIndex = -1;
                    }
                });
        }
    }

    bool AnyWorkersFree => workers.Any(w => w.queueTaskIndex == -1);

    void dispatchWorker(QueueTask task)
    {
        Stonicorn worker = workers.First(w => w.queueTaskIndex == -1);
        worker.queueTaskIndex = queue.IndexOf(task);
        worker.position = Managers.Planet.Planet.getCeilingPos(task.pos);
    }

    void taskCompleted(QueueTask task)
    {
        int index = queue.IndexOf(task);
        queue.Remove(task);
        workers.FindAll(w => w.queueTaskIndex >= index)
            .ForEach(w => w.queueTaskIndex = -1);
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
