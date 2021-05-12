using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    public List<Vector2> edges;
    public List<Vector2> convertEdges;

    // Start is called before the first frame update
    void Awake()
    {
        Managers.Planet.onPodsListChanged += addPod;
        Managers.Planet.onPodContentsListChanged += addPodContent;
        Managers.Queue.onQueueChanged += queueUpdated;
        Managers.Input.onPlanetObjectTypeChanged += calculateConvertEdgesNew;
    }

    public void queueUpdated(List<QueueTask> tasks)
    {
        calculateEdges();
    }
    private void addPod(List<Pod> pods)
    {
        calculateEdges();
    }
    private void addPodContent(List<PodContent> podContents)
    {
        calculateEdges();
    }

    void calculateEdges()
    {
        edges = Managers.Planet.futurePlanet.Border;
        onEdgeListChanged?.Invoke(edges);
        calculateConvertEdgesNew(Managers.Input.PlanetObjectType);
    }
    public delegate void OnPositionListChanged(List<Vector2> edges);
    public event OnPositionListChanged onEdgeListChanged;

    void calculateConvertEdgesNew(PlanetObjectType pot)
    {
        if (pot is PodType pt)
        {
            calculateConvertEdges(pt);
        }
        else if (pot is PodContentType pct)
        {
            calculatePlantEdges(pct);
        }
    }

    void calculateConvertEdges(PodType podType)
    {
        if (podType)
        {
            convertEdges = new List<Vector2>();
            podType.constructFromTypes.ForEach(
                cpt => convertEdges.AddRange(
                    Managers.Planet.futurePlanet.Pods(cpt)
                    .ConvertAll(pod => pod.pos)
                    ));
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
    public event OnPositionListChanged onConvertEdgeListChanged;

    void calculatePlantEdges(PodContentType podContentType)
    {
        if (podContentType)
        {
            convertEdges = new List<Vector2>();
            podContentType.podImplantTypes.ForEach(
                ipt => convertEdges.AddRange(
                    Managers.Planet.futurePlanet.Pods(ipt)
                    .ConvertAll(pod => pod.pos)
                    ));
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
}
