using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : PlanetProcessor
{
    public PodType generatorPodType;
    public float generateRate = 1;
    private int count = 0;

    private void Start()
    {
        Managers.Planet.onPlanetStateChanged += updateCount;
        updateCount(Managers.Planet.Planet);
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        Managers.Resources.Resources += count * generateRate * timeDelta;
    }

    void updateCount(Planet planet)
    {
        count = planet.Pods(generatorPodType).Count;
        enabled = count > 0;
    }
}
