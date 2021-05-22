using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public float defaultWorkRate = 20;

    List<QueueTask> queue = new List<QueueTask>();
    public List<QueueTask> Tasks => queue.ToList();

    List<QueueWorker> workers = new List<QueueWorker>();

    public void addToQueue(QueueTask task)
    {
        if (queue.Contains(task))
        {
            Debug.LogError("Task already in queue");
            return;
        }
        workers.ForEach(w => w.callBack());
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
                    task => !workers.Any(w => w.currentTask == task)
                    ) ?? queue[0];
                //Try to start task
                if (!task.Started
                    && Managers.Planet.Resources >= task.StartCost)
                {
                    Managers.Planet.Resources -= task.StartCost;
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
        }
    }

    public void updateQueueWorkerList(Planet p)
    {
        int queueCount = p.Pods(Managers.Constants.corePodType).Count;
        while (queueCount > workers.Count)
        {
            QueueWorker worker = gameObject.AddComponent<QueueWorker>();
            worker.onTaskCompleted += taskCompleted;
            worker.workSpeed = defaultWorkRate;
            workers.Add(worker);
        }
        while (queueCount < workers.Count)
        {
            QueueWorker worker = workers[0];
            worker.retire();
            workers.Remove(worker);
        }
    }

    bool AnyWorkersFree => workers.Any(w => !w.Busy);

    void dispatchWorker(QueueTask task)
    {
        QueueWorker worker = workers.First(w => !w.Busy);
        worker.dispatch(task);
    }

    void taskCompleted(QueueTask task)
    {
        queue.Remove(task);
        callOnQueueChanged();
        onTaskCompleted?.Invoke(task);
    }
    public delegate void TaskEvent(QueueTask task);
    public event TaskEvent onTaskCompleted;

    public void loadTasks(List<QueueTask> tasks)
    {
        queue = tasks;
        tasks.ForEach(t => t.inflate());
        callOnQueueChanged();
    }
}
