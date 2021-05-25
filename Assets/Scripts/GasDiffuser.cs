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
    public float giveThresholdFactorUp = 0.5f;
    [Range(0, 1)]
    public float giveThresholdFactorDown = 1.0f;

    public PodContentType gasPodContentType;
    public PodType emitterPodType;

    // Update is called once per frame
    void Update()
    {
        bool filledAny = false;

        Managers.Planet.Planet.Pods(Managers.Constants.spacePodType)
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
        if (pod.podType == Managers.Constants.spacePodType)
        {
            diffuseAmount = diffuse(pod.worldPos, currentPressure(pod));
            adjustPressure(pod, -diffuseAmount);
        }
        else if (pod.podType == emitterPodType)
        {
            diffuseAmount = diffuse(pod.worldPos, emitterPressure);
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
                    new Pod(v, Managers.Constants.spacePodType),
                    v
                    )
            );
        Neighborhood<Pod> neighborhood = Managers.Planet.Planet.getNeighborhood(pos);
        List<Pod> spaces = new List<Pod>();
        float diff = Mathf.Abs(giveThresholdFactorUp - giveThresholdFactorDown);
        float min = Mathf.Min(giveThresholdFactorUp, giveThresholdFactorDown);
        if (canDiffuse(
                neighborhood.ceiling,
                curAmount,
                giveThresholdFactorUp
                ))
        {
            spaces.Add(neighborhood.ceiling);
        }
        spaces.AddRange(neighborhood.upsides.ToList()
            .FindAll(pod => canDiffuse(
                pod,
                curAmount,
                ((giveThresholdFactorUp - giveThresholdFactorDown) * 2 / 3) + giveThresholdFactorDown
                )));
        if (canDiffuse(
                neighborhood.ground,
                curAmount,
                giveThresholdFactorDown
                ))
        {
            spaces.Add(neighborhood.ground);
        }
        spaces.AddRange(neighborhood.downsides.ToList()
            .FindAll(pod => canDiffuse(
                pod,
                curAmount,
                ((giveThresholdFactorUp - giveThresholdFactorDown) * 1 / 3) + giveThresholdFactorDown
                )));
        spaces.ForEach(pod => fillWithGas(pod));
        return spaces.Count * diffusionRate * Time.deltaTime;
    }

    bool canDiffuse(Pod pod, float curAmount, float threshold)
        => pod && pod.podType == Managers.Constants.spacePodType
        && currentPressure(pod) < curAmount * threshold;

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
