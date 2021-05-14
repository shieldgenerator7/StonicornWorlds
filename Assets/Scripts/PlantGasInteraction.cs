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
        List<Pod> interactPods = Managers.Planet.Planet.Pods(Managers.PodTypeBank.spacePodType)
            .FindAll(pod => pod.hasContent(plantType) && pod.hasContent(gasType));
        interactPods.ForEach(
            pod => interact(pod.getContent(plantType), pod.getContent(gasType))
            );
    }

    void interact(PodContent plant, PodContent gas)
    {
        plant.Var = Mathf.Clamp(
            plant.Var + (plantHealthDelta * Time.deltaTime),
            0.0f,
            plantType.initialVarValue
            );
        if (plant.Var == 0)
        {
            plant.container.removeContent(plant);
        }
        gas.Var += gasPressureDelta * Time.deltaTime;
        if (gas.Var <= 0)
        {
            gas.container.removeContent(gas);
        }
    }
}
