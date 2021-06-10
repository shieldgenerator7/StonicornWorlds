using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGasInteraction : MonoBehaviour
{
    public PodContentType plantType;
    public PodContentType gasType;
    public float plantHealthDelta;
    public float gasPressureDelta;


    // Update is called once per frame
    void Update()
    {
        List<Pod> interactPods = Managers.Planet.Planet.Pods(Managers.Constants.spacePodType)
            .FindAll(pod => pod.hasContent(plantType) && pod.hasContent(gasType));
        List<PodContent> destroyedPlants = interactPods.FindAll(
            pod => interact(pod.getContent(plantType), pod.getContent(gasType))
            )
            .ConvertAll(pod=>pod.getContent(plantType));
        Managers.Planet.destroyPodContents(destroyedPlants);
    }

    bool interact(PodContent plant, PodContent gas)
    {
        bool plantDestroyed = false;
        plant.Var = Mathf.Clamp(
            plant.Var + (plantHealthDelta * Time.deltaTime),
            0.0f,
            plantType.initialVarValue
            );
        if (plant.Var == 0)
        {
            plantDestroyed = true;
        }
        gas.Var += gasPressureDelta * Time.deltaTime;
        if (gas.Var <= 0)
        {
            Managers.Queue.addToQueue(new QueueTask(
                gas.contentType,
                gas.container.worldPos,
                QueueTask.Type.PLANT
                ));
            gas.container.removeContent(gas);
        }
        return plantDestroyed;
    }
}
