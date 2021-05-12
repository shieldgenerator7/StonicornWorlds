using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkySpreader : MonoBehaviour
{
    public float diffusionRate = 5;
    public float minAmount = 10;//min pressure amount to start diffusing

    public PlanetManager planetManager;
    public PodContentType skyPodContentType;
    public PodType waterPodType;
    public PodType spacePodType;

    // Update is called once per frame
    void Update()
    {
        bool filledAny = false;

        planetManager.planet.Pods(spacePodType)
            .FindAll(pod => currentPressure(pod) >= minAmount)
            .ForEach(pod => filledAny = diffuse(pod) || filledAny);

        planetManager.planet.Pods(waterPodType)
            .ForEach(pod => filledAny = diffuse(pod) || filledAny);

        if (filledAny)
        {
            planetManager.queueManager.callOnQueueChanged();
        }
    }

    bool diffuse(Pod pod)
    {
        float diffuseAmount = 0;
        if (pod.podType == spacePodType)
        {
            diffuseAmount = diffuse(pod.pos, currentPressure(pod));
            adjustPressure(pod, -diffuseAmount);
        }
        else if (pod.podType == waterPodType)
        {
            diffuseAmount = diffuse(pod.pos, 200);
        }
        return diffuseAmount > 0;
    }

    float diffuse(Vector2 pos, float curAmount)
    {
        if (curAmount < minAmount)
        {
            return 0;
        }
        planetManager.planet.getEmptyNeighborhood(pos)
            .ForEach(
                v => planetManager.planet.addPod(
                    new Pod(v, spacePodType),
                    v
                    )
            );
        List<Pod> spaces = planetManager.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod =>
                pod && pod.podType == spacePodType
                && currentPressure(pod) < curAmount / 2
                );
        spaces.ForEach(pod => fillWithAir(pod));
        return spaces.Count * diffusionRate * Time.deltaTime;
    }

    PodContent getSky(Pod pod)
        => pod.podContents.FirstOrDefault(pc => pc.contentType == skyPodContentType);

    float currentPressure(Pod pod)
    {
        PodContent content = getSky(pod);
        if (content)
        {
            return content.Var;
        }
        return 0;
    }

    void adjustPressure(Pod pod, float delta)
    {
        PodContent content = getSky(pod);
        if (content)
        {
            content.Var += delta;
        }
    }

    bool fillWithAir(Pod pod)
    {
        PodContent content = pod.podContents.FirstOrDefault(
            pc => pc.contentType == skyPodContentType
            );
        if (!content)
        {
            content = new PodContent(skyPodContentType, pod);
            pod.podContents.Add(content);
            planetManager.addPodContent(content);
            content.Var = 0;
            return true;
        }
        content.Var += diffusionRate * Time.deltaTime;
        return false;
    }
}
