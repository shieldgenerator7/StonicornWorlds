using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : Manager
{
    public float fastForwardTimeDelta = 0.1f;
    public List<PlanetProcessor> processors;

    public override void setup()
    {
        long now = DateTime.Now.Ticks;
        long lastTime = Managers.Player.Player.lastSavedTicks;
        TimeSpan span = new TimeSpan(now - lastTime);
        float timeLeftToProcess = (float)span.TotalSeconds;
        while (timeLeftToProcess > 0)
        {
            update(fastForwardTimeDelta);
            timeLeftToProcess -= fastForwardTimeDelta;
        }
        Managers.Progression.checkAllProgression();
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
