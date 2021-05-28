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
            //onPlanetSwapped?.Invoke(planet);
            onPlanetStateChanged?.Invoke(planet);
        }
    }
    public delegate void OnPlanetStateChanged(Planet p);
    //public event OnPlanetStateChanged onPlanetSwapped;
    public event OnPlanetStateChanged onPlanetStateChanged;
    //public event OnPlanetStateChanged onFuturePlanetSwapped;
    public event OnPlanetStateChanged onFuturePlanetStateChanged;
    private void planetChanged(Planet p) => onPlanetStateChanged?.Invoke(p);
    private void futurePlanetChanged(Planet p) => onFuturePlanetStateChanged?.Invoke(p);

    private Planet futurePlanet;
    public Planet FuturePlanet
    {
        get => futurePlanet;
        set
        {
            if (futurePlanet != null)
            {
                futurePlanet.onStateChanged -= futurePlanetChanged;
            }
            futurePlanet = value;
            futurePlanet.onStateChanged += futurePlanetChanged;
            //onFuturePlanetSwapped?.Invoke(futurePlanet);
            onFuturePlanetStateChanged?.Invoke(futurePlanet);
        }
    }

    public override void setup()
    {
        if (planet.PodsAll.Count == 0)
        {
            planet.position = Vector2.zero;
            FuturePlanet = planet;
            Pod starter = new Pod(Vector2.zero, Managers.Constants.corePodType);
            addPod(starter);
            planet.residents[0].rest = 500;
            planet.residents[0].resting = false;
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

    public bool canBuildAtPosition(PodType podType, Vector2 pos, bool useFuture = true)
    {
        if (!podType)
        {
            return false;
        }
        Planet canPlanet = (useFuture) ? futurePlanet : planet;
        List<PodType> neighborTypes = getNeighbors(pos, canPlanet)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = canPlanet.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        return podType.areAllNeighborsAllowed(neighborTypes)
            && podType.isRequiredNeighborPresent(neighborTypes)
            && podType.canConvertFrom(curPodType);
    }

    public bool canPlantAtPosition(PodContentType podContentType, Vector2 pos, bool useFuture = true)
    {
        Planet canPlanet = (useFuture) ? futurePlanet : planet;
        List<PodType> neighborTypes = getNeighbors(pos, canPlanet)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = canPlanet.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = canPlanet.getGroundPod(pos);
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

    public List<Pod> getNeighbors(Vector2 pos, Planet p)
    {
        return p.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod);
    }

    #region Predict State
    public void calculateFutureState()
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
        FuturePlanet = fp;
    }
    #endregion
}
