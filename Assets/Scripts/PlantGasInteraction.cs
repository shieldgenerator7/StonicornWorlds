using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGasInteraction : PlanetProcessor
{
    public PodContentType plantType;
    public PodContentType gasType;
    public float plantHealthDelta;
    public float gasPressureDelta;

    HexagonGrid<float> gasGrid;

    private void Start()
    {
        gasGrid = Managers.Planet.Planet.getGasGrid(gasType);
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        if (gasGrid == null)
        {
            return;
        }
        List<Pod> interactPods = Managers.Planet.Planet.Pods(plantType)
            .FindAll(pod => gasGrid[pod.gridPos] > 0 && pod.hasContent(plantType));
        List<PodContent> destroyedPlants = interactPods.FindAll(
            pod => interact(pod.getContent(plantType), pod.gridPos, timeDelta)
            )
            .ConvertAll(pod => pod.getContent(plantType));
        Managers.Planet.destroyPodContents(destroyedPlants);
    }

    bool interact(PodContent plant, Vector3Int v, float timeDelta)
    {
        //Gas
        gasGrid[v] += Mathf.Max(0, gasPressureDelta * timeDelta);
        //Plant
        plant.Var = Mathf.Clamp(
            plant.Var + (plantHealthDelta * timeDelta),
            0.0f,
            plantType.initialVarValue
            );
        //Return true if plant is destroyed
        return plant.Var == 0;
    }
}
