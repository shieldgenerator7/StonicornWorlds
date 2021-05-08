using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkySpreader : MonoBehaviour
{
    public PlanetManager planetManager;
    public PodContentType skyPodContentType;
    public PodType waterPodType;
    public PodType spacePodType;

    // Update is called once per frame
    void Update()
    {
        planetManager.planet.Pods.FindAll(pod => pod.podType == waterPodType)
            .ForEach(pod => fillWithAir(pod.pos));
    }

    void fillWithAir(Vector2 pos)
    {
        List<Pod> spaces = planetManager.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod && pod.podType == spacePodType);
        spaces.FindAll(pod => pod &&
           pod.podContents.Any(pc => pc.contentType == skyPodContentType)
           )
           .ForEach(pod => fillWithAirSecondary(pod.pos));
        spaces.ForEach(pod => fillWithAir(pod));
    }
    void fillWithAirSecondary(Vector2 pos)
    {
        List<Pod> spaces = planetManager.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod && pod.podType == spacePodType);
        spaces.ForEach(pod => fillWithAir(pod));
    }

    void fillWithAir(Pod pod)
    {
        if (!pod.podContents.Any(pc => pc.contentType == skyPodContentType))
        {
            PodContent pc = new PodContent(skyPodContentType, pod);
            pod.podContents.Add(pc);
            planetManager.addPodContent(pc);
        }
    }
}
