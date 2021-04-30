using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public float progressRate = 20;

    public PlanetManager planetManager;

    Queue<Pod> queue = new Queue<Pod>();

    Pod currentPod;

    public void addToQueue(Pod pod)
    {
        queue.Enqueue(pod);
        onQueueChanged?.Invoke(queue);
    }
    public delegate void OnQueueChanged(Queue<Pod> queue);
    public event OnQueueChanged onQueueChanged;


    // Update is called once per frame
    void Update()
    {
        if (!currentPod && queue.Count > 0)
        {
            Pod pod = queue.Peek();
            if (planetManager.Resources >= pod.podType.progressRequired)
            {
                currentPod = pod;
                planetManager.Resources -= currentPod.podType.progressRequired;
            }
        }
        if (currentPod)
        {
            currentPod.Progress += progressRate * Time.deltaTime;
            if (currentPod.Completed)
            {
                onPodCompleted?.Invoke(currentPod);
                queue.Dequeue();
                onQueueChanged?.Invoke(queue);
                currentPod = null;
            }
        }
    }
    public delegate void PodEvent(Pod pod);
    public event PodEvent onPodCompleted;
}