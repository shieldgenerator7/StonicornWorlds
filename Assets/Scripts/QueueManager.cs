using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public PlanetManager planetManager;

    List<Pod> queue = new List<Pod>();

    List<QueueWorker> workers = new List<QueueWorker>();

    public void addToQueue(Pod pod)
    {
        queue.Remove(pod);
        queue.Add(pod);
        callOnQueueChanged();
    }
    private void callOnQueueChanged()
    {
        onQueueChanged?.Invoke(queue);
    }
    public delegate void OnQueueChanged(List<Pod> queue);
    public event OnQueueChanged onQueueChanged;

    private void Awake()
    {
        planetManager.onPodsListChanged += updateQueueWorkerList;
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Count > 0)
        {
            while (AnyWorkersFree)
            {
                Pod pod = queue[0];
                //Try to start pod
                if (pod.Progress == 0
                    && planetManager.Resources >= pod.podType.progressRequired)
                {
                    planetManager.Resources -= pod.podType.progressRequired;
                    pod.Progress = 0.01f;
                }
                //If pod in front of queue has been started,
                if (pod.Progress > 0)
                {
                    //Work on it more
                    dispatchWorker(pod);
                    //Move pod to end of list
                    queue.Remove(pod);
                    queue.Add(pod);
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
        int queueCount = planetManager.CoreCount;
        while (queueCount > workers.Count)
        {
            QueueWorker worker = gameObject.AddComponent<QueueWorker>();
            worker.onPodCompleted += podCompleted;
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

    void dispatchWorker(Pod pod)
    {
        QueueWorker worker = workers.First(w => !w.Busy);
        worker.dispatch(pod);
    }

    void podCompleted(Pod pod)
    {
        queue.Remove(pod);
        callOnQueueChanged();
        onPodCompleted?.Invoke(pod);
    }
    public delegate void PodEvent(Pod pod);
    public event PodEvent onPodCompleted;
}
