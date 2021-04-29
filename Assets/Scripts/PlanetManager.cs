using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public PodType podType;
    public PodType corePodType;

    public QueueManager queueManager;

    List<Pod> pods = new List<Pod>();

    public delegate void PodsListChanged(List<Pod> list);
    public PodsListChanged podsListChanged;


    // Start is called before the first frame update
    void Start()
    {
        Pod starter = new Pod(Vector2.zero, corePodType);
        starter.Completed = true;
        addPod(starter);
        queueManager.onPodCompleted += (pod) => addPod(pod);
    }

    public void addPod(Pod pod)
    {
        if (!pods.Contains(pod))
        {
            pods.Add(pod);
        }
        //Call list changed even if the pod is already in the list
        podsListChanged?.Invoke(pods);
        if (pod.Progress < Pod.PROGRESS_REQUIRED)
        {
            queueManager.addToQueue(pod);
        }
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
}
