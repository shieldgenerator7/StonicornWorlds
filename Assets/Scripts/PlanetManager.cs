using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public float resourceCapPerCore = 700;
    public PodType corePodType;
    [SerializeField]
    private PodType podType;
    public PodType PodType
    {
        get => podType;
        set
        {
            podType = value;
            onPodTypeChanged?.Invoke(podType);
            if (podType)
            {
                PodContentType = null;
            }
        }
    }
    public delegate void OnPodTypeChanged(PodType podType);
    public event OnPodTypeChanged onPodTypeChanged;

    [SerializeField]
    private PodContentType podContentType;
    public PodContentType PodContentType
    {
        get => podContentType;
        set
        {
            podContentType = value;
            onPodContentTypeChanged?.Invoke(podContentType);
            if (podContentType)
            {
                PodType = null;
            }
        }
    }
    public delegate void OnPodContentTypeChanged(PodContentType podType);
    public event OnPodContentTypeChanged onPodContentTypeChanged;

    public QueueManager queueManager;

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
    public int CoreCount => coreCount;

    public Planet planet;
    public Planet futurePlanet { get; set; }
    public List<Pod> Pods => planet.Pods;
    public delegate void OnPodsListChanged(List<Pod> list);
    public event OnPodsListChanged onPodsListChanged;

    List<PodContent> podContents = new List<PodContent>();//generated from pods' contents variable
    public delegate void OnPodContentsListChanged(List<PodContent> list);
    public event OnPodContentsListChanged onPodContentsListChanged;


    // Start is called before the first frame update
    void Start()
    {
        planet = new Planet();
        planet.position = Vector2.zero;
        futurePlanet = planet;
        Pod starter = new Pod(Vector2.zero, corePodType);
        addPod(starter);
        calculateFutureState(new List<QueueTask>());
        queueManager.onTaskCompleted += (task) => updatePlanet(task);
        queueManager.onQueueChanged += (tasks) => calculateFutureState(tasks);
        Application.runInBackground = true;
        Resources = ResourceCap;
    }

    public void addPod(Pod pod)
    {
        planet.addPod(pod, pod.pos);
        List<Pod> pods = Pods;
        coreCount = pods.FindAll(pod =>
            pod.podType == corePodType
            ).Count;
        onPodsListChanged?.Invoke(pods);
        onPodContentsListChanged?.Invoke(podContents);
    }

    public void addPodContent(PodContent podContent)
    {
        podContents.Add(podContent);
        onPodContentsListChanged?.Invoke(podContents);
    }

    public void convertPod(Pod newPod)
    {
        planet.removePod(newPod.pos);
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
                addPodContent(
                    new PodContent(
                        (PodContentType)task.taskObject,
                        planet.getPod(task.pos)
                    ));
                break;
        }
    }

    public bool checkAddPodUI(Vector2 pos1, Vector2 pos2)
    {
        float radius = 0.5f;
        return Vector2.Distance(pos1, pos2) <= radius;
    }

    public Vector2 upDir(Vector2 pos)
    {
        Vector2 ground = planet.getGroundPos(pos);
        return (pos - ground).normalized;
    }

    public bool canBuildAtPosition(PodType podType, Vector2 pos)
    {
        List<PodType> neighborTypes = getFutureNeighbors(pos)
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
        List<PodType> neighborTypes = getFutureNeighbors(pos)
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
            && !(curPod && curPod.podContents.Any(
                content => content.contentType == podContentType
                ));
    }

    public List<Pod> getFutureNeighbors(Vector2 pos)
    {
        return planet.getNeighborhood(pos).neighbors.ToList()
            .FindAll(pod => pod);
    }

    #region Predict State
    public void calculateFutureState(List<QueueTask> queue)
    {
        futurePlanet = planet.deepCopy();
        queue.ForEach(task =>
            {
                switch (task.type)
                {
                    case QueueTask.Type.CONSTRUCT:
                    case QueueTask.Type.CONVERT:
                        futurePlanet.addPod(
                            new Pod(task.pos, (PodType)task.taskObject),
                            task.pos
                            );
                        break;
                    case QueueTask.Type.PLANT:
                        Pod pod = futurePlanet.getPod(task.pos);
                        pod.podContents.Add(
                            new PodContent((PodContentType)task.taskObject, pod)
                            );
                        break;
                    default:
                        Debug.LogError("No case for value: " + task.type);
                        break;
                }
            }
            );
    }
    #endregion
}
