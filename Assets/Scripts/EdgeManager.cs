using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    public PlanetManager planetManager;
    public QueueManager queueManager;

    List<Pod> pods = new List<Pod>();
    List<Vector2> edges;

    // Start is called before the first frame update
    void Awake()
    {
        planetManager.podsListChanged += addPod;
        queueManager.onQueueChanged += addPod;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickedUI = edges.FirstOrDefault(
                edge => planetManager.checkAddPodUI(pos, edge)
                );
            if (clickedUI != Vector2.zero
                && planetManager.canBuildAtPosition(planetManager.PodType, pos))
            {
                Pod pod = new Pod(
                    clickedUI,
                    planetManager.PodType
                    );
                planetManager.addPod(pod);
            }
        }
    }

    private void addPod(List<Pod> pods)
    {
        pods.FindAll(pod => !this.pods.Contains(pod))
            .ForEach(pod => this.pods.Add(pod));
        calculateEdges();
    }

    private void addPod(Queue<Pod> pods)
    {
        pods.ToList()
            .FindAll(pod => !this.pods.Contains(pod))
            .ForEach(pod => this.pods.Add(pod));
        calculateEdges();
    }

    void calculateEdges()
    {
        List<Vector2> posList = pods.ConvertAll(pod => pod.pos);
        edges = new List<Vector2>();
        foreach (Vector2 pos in posList)
        {
            List<Vector2> adjs = planetManager.getAdjacentPositions(pos);
            foreach (Vector2 adj in adjs)
            {
                if (!planetManager.doesListContainPosition(posList, adj, 0.1f)
                && !planetManager.doesListContainPosition(edges, adj, 0.1f))
                {
                    edges.Add(adj);
                }
            }
        }
        onEdgeListChanged?.Invoke(edges);
    }
    public delegate void OnEdgeListChanged(List<Vector2> edges);
    public event OnEdgeListChanged onEdgeListChanged;
}
