using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : Manager
{
    private Planet planet;
    public Planet Planet
    {
        get => planet;
        set
        {
            if (planet != null)
            {
                planet.onStateChanged -= planetChanged;
            }
            planet = value;
            planet.onStateChanged += planetChanged;
            onPlanetStateChanged?.Invoke(planet);
        }
    }
    public delegate void OnPlanetStateChanged(Planet p);
    public event OnPlanetStateChanged onPlanetStateChanged;
    public event OnPlanetStateChanged onPlannedPlanetStateChanged;
    private void planetChanged(Planet p) => onPlanetStateChanged?.Invoke(p);
    private void plannedPlanetChanged(Planet p) => onPlannedPlanetStateChanged?.Invoke(p);

    public Planet PlannedPlanet
    {
        get => planet.plans;
        set
        {
            if (planet.plans != null)
            {
                planet.plans.onStateChanged -= plannedPlanetChanged;
            }
            planet.plans = value;
            planet.plans.onStateChanged += plannedPlanetChanged;
            onPlannedPlanetStateChanged?.Invoke(planet.plans);
        }
    }

    public override void setup()
    {
        if (planet.PodsAll.Count == 0)
        {
            planet.position = Vector2.zero;
            Pod starter = new Pod(Vector2.zero, Managers.Constants.corePodType);
            addPod(starter);
            PlannedPlanet = planet.deepCopy();
            planet.residents[0].rest = 500;
            planet.residents[0].action = Stonicorn.Action.IDLE;
            FindObjectOfType<StonicornGenerator>().statsFromProfile(
                planet.residents[0],
                0
                );
        }
    }

    public void addPod(Pod pod)
    {
        planet.addPod(pod, pod.worldPos);
    }

    public void convertPod(Pod newPod)
    {
        addPod(newPod);
    }

    public void updatePlanet(QueueTask task)
    {
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
                addPod(new Pod(task.pos, (PodType)task.taskObject));
                break;
            case QueueTask.Type.CONVERT:
                convertPod(new Pod(task.pos, (PodType)task.taskObject));
                break;
            case QueueTask.Type.DESTRUCT:
                planet.removePod(task.pos);
                break;
            case QueueTask.Type.PLANT:
                new PodContent(
                    (PodContentType)task.taskObject,
                    planet.getPod(task.pos)
                );
                break;
        }
    }

    public void updatePlans(QueueTask task)
    {
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
                PlannedPlanet.addPod(new Pod(task.pos, (PodType)task.taskObject), task.pos);
                break;
            case QueueTask.Type.CONVERT:
                PlannedPlanet.addPod(new Pod(task.pos, (PodType)task.taskObject), task.pos);
                break;
            case QueueTask.Type.DESTRUCT:
                PlannedPlanet.removePod(task.pos);
                break;
            case QueueTask.Type.PLANT:
                new PodContent(
                    (PodContentType)task.taskObject,
                    PlannedPlanet.getPod(task.pos)
                );
                break;
        }
    }

    public bool canBuildAtPosition(PodType podType, Vector2 pos)
    {
        if (!podType)
        {
            return false;
        }
        List<PodType> neighborTypes = getNeighbors(pos, planet)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = planet.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        return podType.areAllNeighborsAllowed(neighborTypes)
            && podType.isRequiredNeighborPresent(neighborTypes)
            && podType.canConvertFrom(curPodType);
    }

    public bool canPlantAtPosition(PodContentType podContentType, Vector2 pos)
    {
        List<PodType> neighborTypes = getNeighbors(pos, planet)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = planet.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = planet.getGroundPod(pos);
        if (groundPod)
        {
            groundPodType = groundPod.podType;
        }
        return podContentType.hasRequiredGround(groundPodType)
            && podContentType.canPlantIn(curPodType)
            && podContentType.isRequiredNeighborPresent(neighborTypes)
            && podContentType.hasRequiredContent(curPod)
            && !(curPod && curPod.hasContent(podContentType));
    }

    public bool canPlanBuildAtPosition(PodType podType, Vector2 pos)
    {
        if (!podType)
        {
            return false;
        }
        List<PodType> neighborTypes = getNeighbors(pos, planet.plans)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = planet.plans.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        return podType.areAllNeighborsAllowed(neighborTypes)
            && podType.isRequiredNeighborPresent(neighborTypes)
            && podType.canConvertFrom(curPodType);
    }

    public bool canPlanPlantAtPosition(PodContentType podContentType, Vector2 pos)
    {
        List<PodType> neighborTypes = getNeighbors(pos, planet.plans)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = planet.plans.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = planet.plans.getGroundPod(pos);
        if (groundPod)
        {
            groundPodType = groundPod.podType;
        }
        return podContentType.hasRequiredGround(groundPodType)
            && podContentType.canPlantIn(curPodType)
            && podContentType.isRequiredNeighborPresent(neighborTypes)
            && podContentType.hasRequiredPlanContent(curPod)
            && !(curPod && curPod.hasContent(podContentType));
    }

    public List<Pod> getNeighbors(Vector2 pos, Planet p)
    {
        return p.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod);
    }

    #region Predict State
    public void destroyPod(Pod pod)
    {
        if (PlannedPlanet.hasPod(pod.worldPos))
        {
            Managers.Queue.addToQueue(new QueueTask(pod.podType, pod.worldPos, QueueTask.Type.CONSTRUCT));
            pod.forEachContent(
                content => Managers.Queue.addToQueue(new QueueTask(
                    content.contentType,
                    content.container.worldPos,
                    QueueTask.Type.PLANT
                    ))
                );
        }
        planet.removePod(pod);
    }
    public void calculatePlannedState()
    {
        Planet fp = planet.deepCopy();
        planet.tasks.ForEach(task =>
            {
                switch (task.type)
                {
                    case QueueTask.Type.CONSTRUCT:
                    case QueueTask.Type.CONVERT:
                        fp.addPod(
                            new Pod(task.pos, (PodType)task.taskObject),
                            task.pos
                            );
                        break;
                    case QueueTask.Type.PLANT:
                        Pod pod = fp.getPod(task.pos);
                        pod.addContent(
                            new PodContent((PodContentType)task.taskObject, pod)
                            );
                        break;
                    default:
                        Debug.LogError("No case for value: " + task.type);
                        break;
                }
            }
            );
        PlannedPlanet = fp;
    }
    #endregion
}
