using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public float resourceCapPerCore = 700;

    float resources;
    public float Resources
    {
        get => resources;
        set
        {
            resources = Mathf.Clamp(value, 0, ResourceCap);
            onResourcesChanged?.Invoke(resources);
        }
    }
    public delegate void OnResourcesChanged(float resources);
    public event OnResourcesChanged onResourcesChanged;

    public float ResourceCap => CoreCount * resourceCapPerCore;

    private int coreCount = 0;
    private int CoreCount => coreCount;

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

    void Start()
    {
        Planet p = new Planet();
        p.position = Vector2.zero;
        this.Planet = p;
        FuturePlanet = planet;
        Pod starter = new Pod(Vector2.zero, Managers.PodTypeBank.corePodType);
        addPod(starter);
        calculateFutureState(new List<QueueTask>());
        Managers.Queue.onTaskCompleted += (task) => updatePlanet(task);
        Managers.Queue.onQueueChanged += (tasks) => calculateFutureState(tasks);
        onPlanetStateChanged += (p) => calculateFutureState(Managers.Queue.Tasks);
        Resources = ResourceCap;
    }

    public void addPod(Pod pod)
    {
        planet.addPod(pod, pod.pos);
        coreCount = planet.Pods(Managers.PodTypeBank.corePodType).Count;
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

    public bool canBuildAtPosition(PodType podType, Vector2 pos)
    {
        if (!podType)
        {
            return false;
        }
        List<PodType> neighborTypes = getFutureNeighbors(pos)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = futurePlanet.getPod(pos);
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
        List<PodType> neighborTypes = getFutureNeighbors(pos)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = futurePlanet.getPod(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = futurePlanet.getGroundPod(pos);
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

    public List<Pod> getFutureNeighbors(Vector2 pos)
    {
        return futurePlanet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod);
    }

    #region Predict State
    public void calculateFutureState(List<QueueTask> queue)
    {
        Planet fp = planet.deepCopy();
        queue.ForEach(task =>
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
