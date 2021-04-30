using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueWorker : MonoBehaviour
{
    public float constructSpeed = 20;

    public Pod constructPod { get; private set; }

    public bool Busy => constructPod;

    // Update is called once per frame
    void Update()
    {
        if (constructPod)
        {
            constructPod.Progress += constructSpeed * Time.deltaTime;
            if (constructPod.Completed)
            {
                onPodCompleted?.Invoke(constructPod);
                constructPod = null;
            }
        }
    }
    public delegate void OnPodCompleted(Pod pod);
    public event OnPodCompleted onPodCompleted;

    public void dispatch(Pod pod)
    {
        if (Busy)
        {
            FindObjectOfType<QueueManager>().addToQueue(constructPod);
        }
        constructPod = pod;
    }

    public void retire()
    {
        if (Busy)
        {
            FindObjectOfType<QueueManager>().addToQueue(constructPod);
        }
        Destroy(this);
    }
}
