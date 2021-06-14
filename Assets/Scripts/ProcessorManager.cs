using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : Manager
{
    public bool fastForwardOnLoad = true;
    public float fastForwardTimeDelta = 0.1f;
    public List<PlanetProcessor> processors;
    private bool fastForwardFinished = false;

    public override void setup()
    {
        if (fastForwardOnLoad)
        {
            long now = DateTime.Now.Ticks;
            long lastTime = Managers.Player.Player.lastSavedTicks;
            TimeSpan span = new TimeSpan(now - lastTime);
            Debug.Log("processor setup: fastforwarding thru (s): " + span.TotalSeconds);
            Debug.Log("processor setup: fastforwarding thru (m): " + span.TotalMinutes);
            float timeLeftToProcess = (float)span.TotalSeconds;
            StartCoroutine(fastForwardAsynchronously(timeLeftToProcess));
        }
        else
        {
            FastForwardPercentDone = 1;
            fastForwardFinished = true;
            onFastForwardFinished?.Invoke();
        }
    }

    public float FastForwardPercentDone { get; private set; }
    IEnumerator fastForwardAsynchronously(float timeLeftToProcess)
    {
        Debug.Log("fast forward start: time left: " + timeLeftToProcess);
        float timeTotal = timeLeftToProcess;
        FastForwardPercentDone = 1 - (timeLeftToProcess / timeTotal);
        while (timeLeftToProcess > 0)
        {
            update(fastForwardTimeDelta);
            timeLeftToProcess -= fastForwardTimeDelta;
            FastForwardPercentDone = 1 - (timeLeftToProcess / timeTotal);
            Debug.Log("fast forward time left: " + timeLeftToProcess);
            yield return null;
        }
        FastForwardPercentDone = 1;
        fastForwardFinished = true;
        onFastForwardFinished?.Invoke();
    }

    private void fastForward(float timeLeftToProcess)
    {
        while (timeLeftToProcess > 0)
        {
            update(fastForwardTimeDelta);
            timeLeftToProcess -= fastForwardTimeDelta;
        }
        fastForwardFinished = true;
        onFastForwardFinished?.Invoke();
    }
    public delegate void OnFastForwardFinished();
    public event OnFastForwardFinished onFastForwardFinished;

    private void Update()
    {
        if (fastForwardFinished)
        {
            float timeDelta = Time.deltaTime;
            update(timeDelta);
        }
        else
        {
            Debug.Log("Processor update: fastForwardFinishined: " + fastForwardFinished);
        }
    }

    private void update(float timeDelta)
    {
        processors.ForEach(processor => processor.update(timeDelta));
    }
}
