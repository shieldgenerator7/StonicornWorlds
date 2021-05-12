using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkySpreader : MonoBehaviour
{
    public float diffusionRate = 5;
    public float minAmount = 10;//min pressure amount to start diffusing

    public PodContentType skyPodContentType;
    private PodType waterPodType;

    private void Start()
    {
        waterPodType = Managers.PodTypeBank.getPodType("Water");
    }

    // Update is called once per frame
    void Update()
    {
        bool filledAny = false;

        Managers.Planet.planet.Pods(Managers.PodTypeBank.spacePodType)
            .FindAll(pod => currentPressure(pod) >= minAmount)
            .ForEach(pod => filledAny = diffuse(pod) || filledAny);

        Managers.Planet.planet.Pods(waterPodType)
            .ForEach(pod => filledAny = diffuse(pod) || filledAny);

        if (filledAny)
        {
            Managers.Queue.callOnQueueChanged();
        }
    }

    bool diffuse(Pod pod)
    {
        float diffuseAmount = 0;
        if (pod.podType == Managers.PodTypeBank.spacePodType)
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
        Managers.Planet.planet.getEmptyNeighborhood(pos)
            .ForEach(
                v => Managers.Planet.planet.addPod(
                    new Pod(v, Managers.PodTypeBank.spacePodType),
                    v
                    )
            );
        List<Pod> spaces = Managers.Planet.planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod =>
                pod && pod.podType == Managers.PodTypeBank.spacePodType
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
            Managers.Planet.addPodContent(content);
            content.Var = 0;
            return true;
        }
        content.Var += diffusionRate * Time.deltaTime;
        return false;
    }
}
