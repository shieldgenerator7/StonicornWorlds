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
            fastForward(timeLeftToProcess);
        }
        else
        {
            fastForwardFinished = true;
        }
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
    }

    private void update(float timeDelta)
    {
        processors.ForEach(processor => processor.update(timeDelta));
    }
}
