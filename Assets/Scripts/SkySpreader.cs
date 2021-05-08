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
        bool filledAny = false;
        planetManager.planet.Pods.FindAll(pod => pod.podType == waterPodType)
            .ForEach(pod => filledAny = fillWithAir(pod.pos) || filledAny);
        if (filledAny)
        {
            planetManager.queueManager.callOnQueueChanged();
        }
    }

    bool fillWithAir(Vector2 pos)
    {
        bool filledAny = false;
        List<Pod> spaces = planetManager.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod && pod.podType == spacePodType);
        spaces.FindAll(pod => pod &&
           pod.podContents.Any(pc => pc.contentType == skyPodContentType)
           )
           .ForEach(pod => filledAny = fillWithAirSecondary(pod.pos) || filledAny);
        spaces.ForEach(pod => filledAny = fillWithAir(pod) || filledAny);
        return filledAny;
    }
    bool fillWithAirSecondary(Vector2 pos)
    {
        bool filledAny = false;
        List<Pod> spaces = planetManager.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod && pod.podType == spacePodType);
        spaces.ForEach(pod => filledAny = fillWithAir(pod) || filledAny);
        return filledAny;
    }

    bool fillWithAir(Pod pod)
    {
        if (!pod.podContents.Any(pc => pc.contentType == skyPodContentType))
        {
            PodContent pc = new PodContent(skyPodContentType, pod);
            pod.podContents.Add(pc);
            planetManager.addPodContent(pc);
            return true;
        }
        return false;
    }
}
