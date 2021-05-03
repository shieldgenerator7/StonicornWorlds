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
        if (queue.Contains(pod))
        {
            queue.Remove(pod);
        }
        else
        {
            workers.ForEach(w => w.callBack());
        }
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
                if (!pod.Started
                    && planetManager.Resources >= pod.podType.progressRequired)
                {
                    planetManager.Resources -= pod.podType.progressRequired;
                    pod.Progress = 0.01f;
                }
                //Try to find a pod that is already in progress
                if (!pod.Started)
                {
                    pod = queue.FirstOrDefault(p => p.Started) ?? queue[0];
                }
                //If pod has been started,
                if (pod.Started)
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
        queue.RemoveAll(p => !pods.Contains(p));
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
        //If a job is canceled mid-construction, call back that worker
        workers.FindAll(w => !queue.Contains(w.constructPod))
            .ForEach(w => w.callBack());
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
