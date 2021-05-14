using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GasDiffuser : MonoBehaviour
{
    public float diffusionRate = 5;
    public float minAmount = 10;//min pressure amount to start diffusing
    public float emitterPressure = 200;
    [Range(0, 1)]
    public float giveThresholdFactor = 0.5f;

    public PodContentType gasPodContentType;
    public PodType emitterPodType;

    // Update is called once per frame
    void Update()
    {
        bool filledAny = false;

        Managers.Planet.Planet.Pods(Managers.PodTypeBank.spacePodType)
            .FindAll(pod => currentPressure(pod) >= minAmount)
            .ForEach(pod => filledAny = diffuse(pod) || filledAny);

        Managers.Planet.Planet.Pods(emitterPodType)
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
        else if (pod.podType == emitterPodType)
        {
            diffuseAmount = diffuse(pod.pos, emitterPressure);
        }
        return diffuseAmount > 0;
    }

    float diffuse(Vector2 pos, float curAmount)
    {
        if (curAmount < minAmount)
        {
            return 0;
        }
        Managers.Planet.Planet.getEmptyNeighborhood(pos)
            .ForEach(
                v => Managers.Planet.Planet.addPod(
                    new Pod(v, Managers.PodTypeBank.spacePodType),
                    v
                    )
            );
        List<Pod> spaces = Managers.Planet.Planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod =>
                pod && pod.podType == Managers.PodTypeBank.spacePodType
                && currentPressure(pod) < curAmount * giveThresholdFactor
                );
        spaces.ForEach(pod => fillWithGas(pod));
        return spaces.Count * diffusionRate * Time.deltaTime;
    }

    PodContent getGas(Pod pod)
        => pod.getContent(gasPodContentType);

    float currentPressure(Pod pod)
    {
        PodContent content = getGas(pod);
        if (content)
        {
            return content.Var;
        }
        return 0;
    }

    void adjustPressure(Pod pod, float delta)
    {
        PodContent content = getGas(pod);
        if (content)
        {
            content.Var += delta;
        }
    }

    bool fillWithGas(Pod pod)
    {
        PodContent content = getGas(pod);
        if (!content)
        {
            content = new PodContent(gasPodContentType, pod);
            pod.addContent(content);
            content.Var = 0;
            return true;
        }
        content.Var += diffusionRate * Time.deltaTime;
        return false;
    }
}
