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
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        float generateAmount = generateRate * timeDelta;
        Managers.Planet.Planet.Pods(generatorPodType)
            .ForEach(pod =>
            {
                PodContent magmaCore = Managers.Resources.getClosestCore(pod.worldPos);
                magmaCore.Var = Mathf.Clamp(
                    magmaCore.Var + generateAmount,
                    0,
                    Managers.Resources.magmaCapPerCore
                    );
            });
    }

}
