using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    List<QueueTask> queue = new List<QueueTask>();

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

    private void Awake()
    {
        Managers.Planet.onPodsListChanged += updateQueueWorkerList;
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Count > 0)
        {
            while (AnyWorkersFree)
            {
                QueueTask task = queue[0];
                //Try to start task
                if (!task.Started
                    && Managers.Planet.Resources >= task.startCost)
                {
                    Managers.Planet.Resources -= task.startCost;
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
                    //Move pod to end of list
                    queue.Remove(task);
                    queue.Add(task);
                }
                //Else stop, can't do anything
                else
                {
                    break;
                }
            }
        }
    }

    void updateQueueWorkerList(List<Pod> pods)
    {
        int queueCount = Managers.Planet.CoreCount;
        while (queueCount > workers.Count)
        {
            QueueWorker worker = gameObject.AddComponent<QueueWorker>();
            worker.onTaskCompleted += taskCompleted;
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
}
