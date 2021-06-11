using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : Manager
{
    public List<PlanetProcessor> processors;

    public override void setup()
    {
    }

    private void Update()
    {
        float timeDelta = Time.deltaTime;
        processors.ForEach(processor => processor.update(timeDelta));
    }
}
