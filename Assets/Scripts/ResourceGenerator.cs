using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : PlanetProcessor
{
    public PodType generatorPodType;
    public float generateRate = 1;
    private List<Pod> generatorPods;

    private void Start()
    {
        Managers.Planet.onPlanetStateChanged += updateList;
        updateList(Managers.Planet.Planet);
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        if (generatorPods == null)
        {
            updateList(Managers.Planet.Planet);
        }
        float generateAmount = generateRate * timeDelta;
        generatorPods.ForEach(
            pod => Managers.Resources.addResourcesAt(pod.worldPos, generateAmount)
            );
    }

    void updateList(Planet planet)
    {
        generatorPods = Managers.Planet.Planet.Pods(generatorPodType);
    }
}
