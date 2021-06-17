using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : Manager
{
    public bool fastForwardOnLoad = true;
    public float fastForwardTimeDelta = 0.1f;
    public int fastForwardBatchSize = 10;
    public int batchCount = 100;
    public List<PlanetProcessor> processors;

    public override void setup()
    {
        if (fastForwardOnLoad)
        {
            long now = DateTime.Now.Ticks;
            long lastTime = Managers.Player.Player.lastSavedTicks;
            TimeSpan span = new TimeSpan(now - lastTime);
            Debug.Log("processor setup: fastforwarding thru (s): " + span.TotalSeconds);
            if (span.TotalSeconds >= 120)
            {
                Debug.Log("processor setup: fastforwarding thru (m): " + span.TotalMinutes);
                if (span.TotalMinutes >= 120)
                {
                    Debug.Log("processor setup: fastforwarding thru (h): " + span.TotalHours);
                }
            }
            float timeLeftToProcess = Mathf.Floor((float)span.TotalSeconds);
            StartCoroutine(fastForwardAsynchronously(timeLeftToProcess));
        }
        else
        {
            FastForwardPercentDone = 1;
            onFastForwardFinished?.Invoke();
        }
    }

    public float FastForwardPercentDone { get; private set; }
    IEnumerator fastForwardAsynchronously(float timeLeftToProcess)
    {
        float timeTotal = timeLeftToProcess;
        FastForwardPercentDone = 1 - (timeLeftToProcess / timeTotal);
        fastForwardBatchSize = Mathf.FloorToInt(Mathf.Max(1, timeLeftToProcess / batchCount));
        while (timeLeftToProcess > 0)
        {
            for (int i = 0; i < fastForwardBatchSize && timeLeftToProcess > 0; i++)
            {
                update(fastForwardTimeDelta);
                timeLeftToProcess -= fastForwardTimeDelta;
            }
            FastForwardPercentDone = 1 - (timeLeftToProcess / timeTotal);
            yield return null;
        }
        FastForwardPercentDone = 1;
        onFastForwardFinished?.Invoke();
    }

    private void fastForward(float timeLeftToProcess)
    {
        while (timeLeftToProcess > 0)
        {
            update(fastForwardTimeDelta);
            timeLeftToProcess -= fastForwardTimeDelta;
        }
        onFastForwardFinished?.Invoke();
    }
    public delegate void OnFastForwardFinished();
    public event OnFastForwardFinished onFastForwardFinished;

    public override void update(float timeDelta)
    {
        processors.ForEach(processor => processor.update(timeDelta));
    }
}
