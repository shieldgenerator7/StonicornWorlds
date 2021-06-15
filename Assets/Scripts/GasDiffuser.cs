using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GasDiffuser : PlanetProcessor
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
    float upsideThreshold = 0;
    float downsideThreshold = 0;
    float diffusionDelta;

    private void Start()
    {
        Managers.Planet.Planet.pullGasGrid(gasPodContentType, emitterPodType, emitterPressure);
        grid = Managers.Planet.Planet.getGasGrid(gasPodContentType);
        //
        float diff = Mathf.Abs(giveThresholdFactorUp - giveThresholdFactorDown);
        float min = Mathf.Min(giveThresholdFactorUp, giveThresholdFactorDown);
        upsideThreshold = (diff * 2 / 3) + min;
        downsideThreshold = (diff * 1 / 3) + min;
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        diffusionDelta = diffusionRate * timeDelta;

        grid.Keys
            .FindAll(v => grid[v] >= minAmount)
            .ForEach(v => diffuseFrom(v));
    }

    void diffuseFrom(Vector3Int v)
    {
        float curAmount = grid[v];
        //Determine which neighbors get diffused into
        HexagonNeighborhood neighborhood = HexagonUtility.getNeighborhood(v);
        List<Vector3Int> spaces = new List<Vector3Int>();
        if (canDiffuseInto(
                neighborhood.ceiling,
                curAmount,
                giveThresholdFactorUp
                ))
        {
            spaces.Add(neighborhood.ceiling);
        }
        spaces.AddRange(neighborhood.upsides.ToList()
            .FindAll(v2 => canDiffuseInto(
                v2,
                curAmount,
                upsideThreshold
                )));
        if (canDiffuseInto(
                neighborhood.ground,
                curAmount,
                giveThresholdFactorDown
                ))
        {
            spaces.Add(neighborhood.ground);
        }
        spaces.AddRange(neighborhood.downsides.ToList()
            .FindAll(v2 => canDiffuseInto(
                v2,
                curAmount,
                downsideThreshold
                )));
        //Diffuse gas into neighbors
        spaces.ForEach(v2 => grid[v2] += diffusionDelta);
        //Take gas from origin
        if (grid[v] != emitterPressure)
        {
            grid[v] -= spaces.Count * diffusionDelta;
        }
    }

    bool canDiffuseInto(Vector3Int v, float curAmount, float threshold)
        => grid[v] >= 0
        && grid[v] < curAmount * threshold;
}
