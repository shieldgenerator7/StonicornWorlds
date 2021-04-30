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
        if (Busy)
        {
            constructPod.Progress += constructSpeed * Time.deltaTime;
            if (constructPod.Completed)
            {
                onPodCompleted?.Invoke(constructPod);
                callBack();
            }
        }
    }
    public delegate void OnPodCompleted(Pod pod);
    public event OnPodCompleted onPodCompleted;

    public void dispatch(Pod pod)
    {
        constructPod = pod;
    }

    public void callBack()
    {
        constructPod = null;
    }

    public void retire()
    {
        Destroy(this);
    }
}
