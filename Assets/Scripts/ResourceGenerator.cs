using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
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
    void Update()
    {
        Managers.Planet.Resources += count * generateRate * Time.deltaTime;
    }

    void updateCount(Planet planet)
    {
        count = planet.Pods(generatorPodType).Count;
        enabled = count > 0;
    }
}
