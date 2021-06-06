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

    HexagonGrid<float> grid = new HexagonGrid<float>();
    float upsideThreshold;
    float downsideThreshold;
    float diffusionDelta;

    // Update is called once per frame
    void Update()
    {
        bool filledAny = false;

        float diff = Mathf.Abs(giveThresholdFactorUp - giveThresholdFactorDown);
        float min = Mathf.Min(giveThresholdFactorUp, giveThresholdFactorDown);
        upsideThreshold = (diff * 2 / 3) + min;
        downsideThreshold = (diff * 1 / 3) + min;
        diffusionDelta = diffusionRate * Time.deltaTime;

        grid = PressureGrid;
        grid.Keys
            .FindAll(v => grid[v] >= minAmount)
            .ForEach(v => filledAny = diffuse(v) || filledAny);
        PressureGrid = grid;

        if (filledAny)
        {
            Managers.Queue.callOnQueueChanged();
        }
    }

    HexagonGrid<float> PressureGrid
    {
        get
        {
            HexagonGrid<float> grid = new HexagonGrid<float>();
            Managers.Planet.Planet.PodsAll
                .ForEach(pod =>
                {
                    float pressure = 0;
                    //Valid place for gas to be
                    if (pod.podType == Managers.Constants.spacePodType)
                    {
                        PodContent content = pod.getContent(gasPodContentType);
                        if (content)
                        {
                            pressure = content.Var;
                        }
                        else
                        {
                            pressure = 0;
                        }
                    }
                    //Gas emitter
                    else if (pod.podType == emitterPodType)
                    {
                        pressure = emitterPressure;
                    }
                    //Invalid place for gas to be
                    else
                    {
                        pressure = -1;
                    }
                    grid.add(pod.gridPos, pressure);
                });
            return grid;
        }
        set
        {
            HexagonGrid<float> grid = value;
            int count = 0;
            grid.Keys
                .FindAll(v => grid[v] >= 0)
                .ForEach(
                v =>
                {
                    Pod pod = Managers.Planet.Planet.getPod(v);
                    if (!pod && grid[v] > 0)
                    {
                        pod = Managers.Planet.Planet.fillSpace(v);
                    }
                    if (pod)
                    {
                        PodContent content = pod.getContent(gasPodContentType);
                        if (!content && grid[v] > 0)
                        {
                            content = new PodContent(gasPodContentType, pod);
                            pod.addContent(content);
                        }
                        if (content)
                        {
                            content.Var = grid[v];
                            count++;
                        }
                    }
                }
                );
        }
    }


    bool diffuse(Vector3Int v)
    {
        float diffuseAmount = diffuse(v, grid[v]);
        grid[v] += -diffuseAmount;
        return diffuseAmount > 0;
    }
    float diffuse(Vector3Int v, float curAmount)
    {
        HexagonNeighborhood neighborhood = HexagonUtility.getNeighborhood(v);
        List<Vector3Int> spaces = new List<Vector3Int>();
        if (canDiffuse(
                neighborhood.ceiling,
                curAmount,
                giveThresholdFactorUp
                ))
        {
            spaces.Add(neighborhood.ceiling);
        }
        spaces.AddRange(neighborhood.upsides.ToList()
            .FindAll(v2 => canDiffuse(
                v2,
                curAmount,
                upsideThreshold
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
            .FindAll(v2 => canDiffuse(
                v2,
                curAmount,
                downsideThreshold
                )));
        spaces.ForEach(v2 => grid[v2] += diffusionDelta);
        return spaces.Count * diffusionDelta;
    }

    bool canDiffuse(Vector3Int v, float curAmount, float threshold)
        => grid[v] >= 0
        && grid[v] < curAmount * threshold;
}
