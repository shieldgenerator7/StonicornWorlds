using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    public PlanetManager planetManager;
    public QueueManager queueManager;

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
                && planetManager.PodType
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
        edges = planetManager.futurePlanet.Border;
        onEdgeListChanged?.Invoke(edges);
        calculcateConvertEdges(planetManager.PodType);
    }
    public delegate void OnPositionListChanged(List<Vector2> edges);
    public event OnPositionListChanged onEdgeListChanged;

    void calculcateConvertEdges(PodType podType)
    {
        if (podType)
        {
            convertEdges = new List<Vector2>();
            podType.constructFromTypes.ForEach(
                cpt => convertEdges.AddRange(
                    planetManager.futurePlanet.Pods(cpt)
                    .ConvertAll(pod => pod.pos)
                    ));
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
    public event OnPositionListChanged onConvertEdgeListChanged;

    void calculcatePlantEdges(PodContentType podContentType)
    {
        if (podContentType)
        {
            convertEdges = new List<Vector2>();
            podContentType.podImplantTypes.ForEach(
                ipt => convertEdges.AddRange(
                    planetManager.futurePlanet.Pods(ipt)
                    .ConvertAll(pod => pod.pos)
                    ));
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
}
