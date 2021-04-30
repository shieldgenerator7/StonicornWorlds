using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public PlanetManager planetManager;

    Queue<Pod> queue = new Queue<Pod>();

    List<QueueWorker> workers = new List<QueueWorker>();

    public void addToQueue(Pod pod)
    {
        queue.Enqueue(pod);
        callOnQueueChanged();
    }
    private void callOnQueueChanged()
    {
        if (onQueueChanged != null)
        {
            List<Pod> pods = new List<Pod>();
            pods.AddRange(queue.ToList());
            workers.ConvertAll(w => w.constructPod)
                .ForEach(pod =>
                {
                    if (pod != null && !pods.Contains(pod))
                    {
                        pods.Add(pod);
                    }
                });
            onQueueChanged(pods);
        }
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
            Pod pod = queue.Peek();
            if (planetManager.Resources >= pod.podType.progressRequired)
            {
                if (dispatchWorker(pod))
                {
                    planetManager.Resources -= pod.podType.progressRequired;
                    queue.Dequeue();
                    callOnQueueChanged();
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

    bool dispatchWorker(Pod pod)
    {
        QueueWorker worker = workers.FirstOrDefault(w => !w.Busy);
        if (worker)
        {
            worker.dispatch(pod);
            return true;
        }
        return false;
    }

    void podCompleted(Pod pod)
    {
        onPodCompleted?.Invoke(pod);
    }
    public delegate void PodEvent(Pod pod);
    public event PodEvent onPodCompleted;
}
