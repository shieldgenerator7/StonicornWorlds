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

    List<Pod> pods = new List<Pod>();
    public delegate void OnPodsListChanged(List<Pod> list);
    public event OnPodsListChanged onPodsListChanged;

    List<PodContent> podContents = new List<PodContent>();//generated from pods' contents variable
    public delegate void OnPodContentsListChanged(List<PodContent> list);
    public event OnPodContentsListChanged onPodContentsListChanged;


    // Start is called before the first frame update
    void Start()
    {
        Pod starter = new Pod(Vector2.zero, corePodType);
        starter.Completed = true;
        addPod(starter);
        queueManager.onPodCompleted += (pod) => addPod(pod);
        Application.runInBackground = true;
        Resources = ResourceCap;
    }

    public void addPod(Pod pod)
    {
        if (!pods.Contains(pod))
        {
            pods.Add(pod);
            coreCount = pods.FindAll(pod =>
                pod.podType == corePodType && pod.Completed
                ).Count;
        }
        //Call list changed even if the pod is already in the list
        onPodsListChanged?.Invoke(pods);
        onPodContentsListChanged?.Invoke(podContents);
        //If it's not complete, add it to build queue
        if (!pod.Completed)
        {
            queueManager.addToQueue(pod);
        }
    }

    public void addPodContent(PodContent podContent)
    {
        podContents.Add(podContent);
        onPodContentsListChanged?.Invoke(podContents);
    }

    public void convertPod(Pod newPod)
    {
        Pod oldPod = pods.Find(p => p.pos == newPod.pos);
        pods.Remove(oldPod);
        addPod(newPod);
    }

    public List<Vector2> getAdjacentPositions(Vector2 pos)
    {
        List<Vector2> dirs = new List<Vector2>()
        {
            new Vector2(0, 0.87f),
            new Vector2(0, -0.87f),
            new Vector2(0.755f, 0.435f),
            new Vector2(-0.755f, -0.435f),
            new Vector2(-0.755f, 0.435f),
            new Vector2(0.755f, -0.435f),
        };
        return dirs.ConvertAll(dir => dir + pos);
    }

    public bool checkAddPodUI(Vector2 pos1, Vector2 pos2)
    {
        float radius = 0.5f;
        return Vector2.Distance(pos1, pos2) <= radius;
    }

    public bool doesListContainPosition(List<Vector2> list, Vector2 pos, float radius)
    {
        return list.Any(v => Vector2.Distance(v, pos) <= radius);
    }

    public Vector2 upDir(Vector2 pos)
    {
        Vector2 ground = groundPos(pos);
        return (pos - ground).normalized;
    }

    public Vector2 groundPos(Vector2 pos)
    {
        Vector2 origin = Vector2.zero;
        return getAdjacentPositions(pos).Aggregate(
            (best, cur) =>
                (Vector2.Distance(cur, origin) < Vector2.Distance(best, origin))
                    ? cur
                    : best
            );
    }

    public PodNeighborhood getNeighborhood(Vector2 pos)
    {
        PodNeighborhood neighborhood = new PodNeighborhood();
        Vector2 gpos = groundPos(pos);
        Vector2 dir = gpos - pos;
        float angleUnit = Mathf.PI / 3;
        neighborhood.ground = getPodAtPosition(gpos);
        neighborhood.groundLeft = getPodAtPosition(pos + rotateDirection(dir, -angleUnit));
        neighborhood.groundRight = getPodAtPosition(pos + rotateDirection(dir, angleUnit));
        neighborhood.ceiling = getPodAtPosition(pos - dir);
        neighborhood.ceilingLeft = getPodAtPosition(pos + rotateDirection(-dir, angleUnit));
        neighborhood.ceilingRight = getPodAtPosition(pos + rotateDirection(-dir, -angleUnit));
        return neighborhood;
    }

    /// <summary>
    /// Returns the given vector rotated by the given angle in radians
    /// </summary>
    /// <param name="dir">The vector to rotate</param>
    /// <param name="angle">The angle of rotation in radians</param>
    /// <returns></returns>
    public Vector2 rotateDirection(Vector2 dir, float angle)
    {
        //2020-05-03: written with help from https://stackoverflow.com/a/14609567/2336212
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        return new Vector2(
            (dir.x * cos) - (dir.y * sin),
            (dir.x * sin) + (dir.y * cos)
            ); ;
    }


    public bool canBuildAtPosition(PodType podType, Vector2 pos)
    {
        List<PodType> neighborTypes = getNeighbors(pos)
            .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = getPodAtPosition(pos);
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
        List<PodType> neighborTypes = getNeighbors(pos)
               .ConvertAll(pod => pod.podType);
        PodType curPodType = null;
        Pod curPod = getPodAtPosition(pos);
        if (curPod)
        {
            curPodType = curPod.podType;
        }
        PodType groundPodType = null;
        Pod groundPod = getPodAtPosition(groundPos(pos));
        if (groundPod)
        {
            groundPodType = groundPod.podType;
        }
        return podContentType.hasRequiredGround(groundPodType)
            && podContentType.canPlantIn(curPodType)
            && podContentType.isRequiredNeighborPresent(neighborTypes)
            && !(curPod && curPod.podContents.Any(
                content => content.contentType == podContentType
                ));
    }

    public Pod getPodAtPosition(Vector2 pos)
    {
        float radius = 0.5f;
        return pods.FirstOrDefault(pod => Vector2.Distance(pod.pos, pos) <= radius);
    }

    public List<Pod> getNeighbors(Vector2 pos)
    {
        float radius = 1;
        return pods.FindAll(pod => Vector2.Distance(pod.pos, pos) <= radius);
    }
}
