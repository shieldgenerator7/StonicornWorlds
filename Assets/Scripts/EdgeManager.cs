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
    List<Vector2> convertEdges;

    // Start is called before the first frame update
    void Awake()
    {
        planetManager.onPodsListChanged += addPod;
        planetManager.onPodContentsListChanged += addPodContent;
        queueManager.onQueueChanged += queueUpdated;
        planetManager.onPodTypeChanged += calculcateConvertEdges;
        planetManager.onPodContentTypeChanged += calculcatePlantEdges;
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
                queueManager.addToQueue(
                    new QueueTask(
                        planetManager.PodType,
                        clickedUI,
                        QueueTask.Type.CONSTRUCT
                        )
                    );
                queueUpdated(null);
            }
            else
            {
                clickedUI = convertEdges.FirstOrDefault(
                    convertEdge => planetManager.checkAddPodUI(pos, convertEdge)
                );
                if (clickedUI != Vector2.zero)
                {
                    if (planetManager.PodType)
                    {
                        if (planetManager.canBuildAtPosition(planetManager.PodType, pos))
                        {
                            queueManager.addToQueue(
                                new QueueTask(
                                    planetManager.PodType,
                                    clickedUI,
                                    QueueTask.Type.CONVERT
                                    )
                                );
                            queueUpdated(null);
                        }
                    }
                    else if (planetManager.PodContentType)
                    {
                        if (planetManager.canPlantAtPosition(planetManager.PodContentType, pos))
                        {
                            queueManager.addToQueue(
                                new QueueTask(
                                    planetManager.PodContentType,
                                    clickedUI,
                                    QueueTask.Type.PLANT
                                    )
                                );
                        }
                    }
                }
            }
        }
    }
    public void queueUpdated(List<QueueTask> tasks)
    {
        addPod(planetManager.Pods);
    }
    private void addPod(List<Pod> pods)
    {
        this.pods = queueManager.getFutureState(pods);
        //pods.FindAll(pod => !this.pods.Contains(pod))
        //    .ForEach(pod => this.pods.Add(pod));
        calculateEdges();
    }
    private void addPodContent(List<PodContent> podContents)
    {
        calculateEdges();
    }

    void calculateEdges()
    {
        edges = planetManager.planet.Border;
        onEdgeListChanged?.Invoke(edges);
        calculcateConvertEdges(planetManager.PodType);
    }
    public delegate void OnPositionListChanged(List<Vector2> edges);
    public event OnPositionListChanged onEdgeListChanged;

    void calculcateConvertEdges(PodType podType)
    {
        if (podType)
        {
            convertEdges = pods
                .FindAll(pod => podType.constructFromTypes.Contains(pod.podType))
                .ConvertAll(pod => pod.pos);
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
    public event OnPositionListChanged onConvertEdgeListChanged;

    void calculcatePlantEdges(PodContentType podContentType)
    {
        if (podContentType)
        {
            convertEdges = pods
                .FindAll(pod => podContentType.podImplantTypes.Contains(pod.podType))
                .ConvertAll(pod => pod.pos);
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
}
