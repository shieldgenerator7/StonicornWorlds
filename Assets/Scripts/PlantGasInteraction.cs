using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGasInteraction : PlanetProcessor
{
    public PodContentType plantType;
    public PodContentType gasType;
    public float plantHealthDelta;
    public float gasPressureDelta;


    // Update is called once per frame
    public override void update(float timeDelta)
    {
        List<Pod> interactPods = Managers.Planet.Planet.Pods(Managers.Constants.spacePodType)
            .FindAll(pod => pod.hasContent(plantType) && pod.hasContent(gasType));
        List<PodContent> destroyedPlants = interactPods.FindAll(
            pod => interact(pod.getContent(plantType), pod.getContent(gasType), timeDelta)
            )
            .ConvertAll(pod=>pod.getContent(plantType));
        Managers.Planet.destroyPodContents(destroyedPlants);
    }

    bool interact(PodContent plant, PodContent gas, float timeDelta)
    {
        bool plantDestroyed = false;
        plant.Var = Mathf.Clamp(
            plant.Var + (plantHealthDelta * timeDelta),
            0.0f,
            plantType.initialVarValue
            );
        if (plant.Var == 0)
        {
            plantDestroyed = true;
        }
        gas.Var += gasPressureDelta * timeDelta;
        if (gas.Var <= 0)
        {
            gas.container.removeContent(gas);
        }
        return plantDestroyed;
    }
}
