using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public float progressRate = 20;

    Queue<Pod> queue = new Queue<Pod>();

    float currentProgress = 0;

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
        if (queue.Count > 0)
        {
            currentProgress += progressRate * Time.deltaTime;
            if (currentProgress >= 100)
            {
                currentProgress = 0;
                Pod pod = queue.Dequeue();
                onPodCompleted?.Invoke(pod);
                onQueueChanged?.Invoke(queue);
            }
        }
    }
    public delegate void OnPodCompleted(Pod pod);
    public event OnPodCompleted onPodCompleted;
}
