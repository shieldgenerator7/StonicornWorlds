using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    List<Vector2> edges;
    List<Vector2> convertEdges;

    // Start is called before the first frame update
    void Awake()
    {
        Managers.Planet.onPodsListChanged += addPod;
        Managers.Planet.onPodContentsListChanged += addPodContent;
        Managers.Queue.onQueueChanged += queueUpdated;
        Managers.Planet.onPodTypeChanged += calculcateConvertEdges;
        Managers.Planet.onPodContentTypeChanged += calculcatePlantEdges;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickedUI = edges.FirstOrDefault(
                edge => Managers.Planet.checkAddPodUI(pos, edge)
                );
            if (clickedUI != Vector2.zero
                && Managers.Planet.PodType
                && Managers.Planet.canBuildAtPosition(Managers.Planet.PodType, pos))
            {
                Managers.Queue.addToQueue(
                    new QueueTask(
                        Managers.Planet.PodType,
                        clickedUI,
                        QueueTask.Type.CONSTRUCT
                        )
                    );
                queueUpdated(null);
            }
            else
            {
                clickedUI = convertEdges.FirstOrDefault(
                    convertEdge => Managers.Planet.checkAddPodUI(pos, convertEdge)
                );
                if (clickedUI != Vector2.zero)
                {
                    if (Managers.Planet.PodType)
                    {
                        if (Managers.Planet.canBuildAtPosition(Managers.Planet.PodType, pos))
                        {
                            Managers.Queue.addToQueue(
                                new QueueTask(
                                    Managers.Planet.PodType,
                                    clickedUI,
                                    QueueTask.Type.CONVERT
                                    )
                                );
                            queueUpdated(null);
                        }
                    }
                    else if (Managers.Planet.PodContentType)
                    {
                        if (Managers.Planet.canPlantAtPosition(Managers.Planet.PodContentType, pos))
                        {
                            Managers.Queue.addToQueue(
                                new QueueTask(
                                    Managers.Planet.PodContentType,
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
        edges = Managers.Planet.futurePlanet.Border;
        onEdgeListChanged?.Invoke(edges);
        calculcateConvertEdges(Managers.Planet.PodType);
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
                    Managers.Planet.futurePlanet.Pods(cpt)
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
                    Managers.Planet.futurePlanet.Pods(ipt)
                    .ConvertAll(pod => pod.pos)
                    ));
        }
        onConvertEdgeListChanged?.Invoke(convertEdges);
    }
}
