using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : Manager
{
    public bool fastForwardOnLoad = true;
    public float fastForwardTimeDelta = 0.1f;
    public List<PlanetProcessor> processors;

    public override void setup()
    {
        if (fastForwardOnLoad)
        {
            long now = DateTime.Now.Ticks;
            long lastTime = Managers.Player.Player.lastSavedTicks;
            TimeSpan span = new TimeSpan(now - lastTime);
            float timeLeftToProcess = (float)span.TotalSeconds;
            fastForward(timeLeftToProcess);
        }
    }

    private void fastForward(float timeLeftToProcess)
    {
        while (timeLeftToProcess > 0)
        {
            update(fastForwardTimeDelta);
            timeLeftToProcess -= fastForwardTimeDelta;
        }
        FindObjectOfType<GameManager>().screenChangedLastFrame = true;
    }

    private void Update()
    {
        float timeDelta = Time.deltaTime;
        update(timeDelta);
    }

    private void update(float timeDelta)
    {
        processors.ForEach(processor => processor.update(timeDelta));
    }
}
