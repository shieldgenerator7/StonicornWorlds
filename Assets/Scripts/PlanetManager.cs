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
    public event OnPlanetStateChanged onPlanetStateChangedUnplanned;
    private void planetChanged(Planet p) => onPlanetStateChanged?.Invoke(p);


    public override void setup()
    {
        if (planet.PodsAll.Count == 0)
        {
            planet.position = Vector2.zero;
            Pod starter = new Pod(Vector2.zero, Managers.Constants.corePodType);
            addPod(starter);
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
        planet.updatePlanet(task);
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
            && podContentType.hasRoomFor(curPod);
    }

    public bool canPlanBuildAtPosition(PodType podType, Vector2 pos)
    {
        if (!podType)
        {
            return false;
        }
        Planet plans = Managers.Queue.plans;
        List<PodType> neighborTypes = getNeighbors(pos, plans)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = plans.getPod(pos);
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
        Planet plans = Managers.Queue.plans;
        List<PodType> neighborTypes = getNeighbors(pos, plans)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = plans.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = plans.getGroundPod(pos);
        if (groundPod)
        {
            groundPodType = groundPod.podType;
        }
        return podContentType.hasRequiredGround(groundPodType)
            && podContentType.canPlantIn(curPodType)
            && podContentType.isRequiredNeighborPresent(neighborTypes)
            && podContentType.hasRequiredPlanContent(curPod)
            && podContentType.hasRoomFor(curPod);
    }

    public List<Pod> getNeighbors(Vector2 pos, Planet p)
    {
        return p.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod);
    }

    public void destroyPods(List<Pod> pods)
    {
        pods.FindAll(pod => pod)
            .ForEach(pod => planet.removePod(pod));
        onPlanetStateChangedUnplanned(planet);
    }
    public void destroyPodContents(List<PodContent> contents)
    {
        contents.FindAll(content => content)
            .ForEach(content => content.container.removeContent(content));
        onPlanetStateChangedUnplanned(planet);
    }
    public void createPods(List<Pod> pods)
    {
        pods.FindAll(pod => pod)
            .ForEach(pod => planet.addPod(pod, pod.worldPos));
        onPlanetStateChangedUnplanned(planet);
    }
}
