using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueWorker : MonoBehaviour
{
    public float workSpeed = 20;

    public QueueTask currentTask { get; private set; }

    public bool Busy => currentTask;

    // Update is called once per frame
    void Update()
    {
        if (Busy)
        {
            currentTask.Progress += workSpeed * Time.deltaTime;
            if (currentTask.Completed)
            {
                onTaskCompleted?.Invoke(currentTask);
                callBack();
            }
        }
    }
    public delegate void OnTaskCompleted(QueueTask task);
    public event OnTaskCompleted onTaskCompleted;

    public void dispatch(QueueTask task)
    {
        currentTask = task;
    }

    public void callBack()
    {
        currentTask = null;
    }

    public void retire()
    {
        Destroy(this);
    }
}
