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
    List<Vector2> addPosList = new List<Vector2>();

    public delegate void AddPosListChanged(List<Vector2> list);
    public AddPosListChanged addPosListChanged;
    public delegate void PodsListChanged(List<Pod> list);
    public PodsListChanged podsListChanged;


    // Start is called before the first frame update
    void Start()
    {
        podsListChanged += updateEdges;
        pods.Add(new Pod(Vector2.zero, corePodType));
        podsListChanged?.Invoke(pods);
        queueManager.onPodCompleted += (pod) =>
        {
            pods.Add(pod);
            podsListChanged?.Invoke(pods);
        };
    }

    public void updateEdges(List<Pod> list)
    {
        List<Vector2> posList = list.ConvertAll(pod => pod.pos);
        addPosList = new List<Vector2>();
        foreach (Vector2 pos in posList)
        {
            List<Vector2> adjs = getAdjacentPositions(pos);
            foreach (Vector2 adj in adjs)
            {
                if (!doesListContainPosition(posList, adj, 0.1f)
                && !doesListContainPosition(addPosList, adj, 0.1f))
                {
                    addPosList.Add(adj);
                }
            }
        }
        addPosListChanged?.Invoke(addPosList);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickedUI = addPosList.FirstOrDefault(addPos => checkAddPodUI(pos, addPos));
            if (clickedUI != Vector2.zero || addPosList.Count == 1)
            {
                Pod pod = new Pod(
                    clickedUI,
                    podType
                    );
                queueManager.addToQueue(pod);
            }
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
