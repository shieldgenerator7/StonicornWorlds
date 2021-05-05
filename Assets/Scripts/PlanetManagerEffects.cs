using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public PlanetManager planetManager;
    public EdgeManager edgeManager;

    public GameObject addPodPrefab;
    public GameObject convertPodPrefab;
    public GameObject plantPodPrefab;

    public TMP_Text txtResources;

    List<GameObject> goPods = new List<GameObject>();
    List<GameObject> goPodContents = new List<GameObject>();
    List<GameObject> addPods = new List<GameObject>();
    List<GameObject> convertPods = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        planetManager.onPodsListChanged += updateDisplay;
        planetManager.onPodContentsListChanged += updateDisplay;
        planetManager.onPodTypeChanged += updateEdgeTypes;
        planetManager.onPodContentTypeChanged += updatePlantTypes;
        planetManager.onResourcesChanged += updateUI;
        edgeManager.onEdgeListChanged += updateAddDisplay;
        edgeManager.onConvertEdgeListChanged += updateConvertDisplay;
    }

    public void updateDisplay(List<Pod> pods)
    {
        //Update pods
        goPods.ForEach(go => Destroy(go));
        goPods.Clear();
        pods.FindAll(pod => pod.Completed)
            .ForEach(pod =>
            {
                GameObject go = Instantiate(
                    pod.podType.podPrefab,
                    pod.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = planetManager.upDir(go.transform.position);
                goPods.Add(go);
            });
    }
    public void updateDisplay(List<PodContent> podContents)
    {
        //Update pod contents
        goPodContents.ForEach(go => Destroy(go));
        goPodContents.Clear();
        podContents.FindAll(content => content.container.Completed)
            .ForEach(content =>
            {
                GameObject go = Instantiate(
                    content.contentType.podContentPrefab,
                    content.container.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = planetManager.upDir(go.transform.position);
                goPodContents.Add(go);
            });
    }
    public void updateAddDisplay(List<Vector2> edges)
    {
        //Update add pod UI
        addPods.ForEach(go => Destroy(go));
        addPods.Clear();
        edges.ForEach(edge =>
        {
            GameObject addPod = Instantiate(
                addPodPrefab,
                edge,
                Quaternion.identity,
                transform
                );
            addPods.Add(addPod);
        });
        updateEdgeTypes(planetManager.PodType);
    }

    void updateEdgeTypes(PodType podType)
    {
        if (podType)
        {
            Color color = podType.uiColor;
            addPods.ForEach(add =>
            {
                add.SetActive(planetManager.canBuildAtPosition(
                    podType,
                    add.transform.position
                    ));
                add.GetComponent<SpriteRenderer>().color = color;
            });
            convertPods.ForEach(convert =>
            {
                convert.SetActive(planetManager.canBuildAtPosition(
                    podType,
                    convert.transform.position
                    ));
                convert.GetComponent<SpriteRenderer>().color = color;
            });
        }
        else
        {
            addPods.ForEach(add => add.SetActive(false));
        }
    }

    void updateConvertDisplay(List<Vector2> convertEdges)
    {
        //Update convert pod UI
        convertPods.ForEach(go => Destroy(go));
        convertPods.Clear();
        convertEdges.ForEach(edge =>
        {
            GameObject convertPod = Instantiate(
                convertPodPrefab,
                edge,
                Quaternion.identity,
                transform
                );
            convertPods.Add(convertPod);
        });
        updateEdgeTypes(planetManager.PodType);
        updatePlantTypes(planetManager.PodContentType);
    }

    void updatePlantTypes(PodContentType podContentType)
    {
        if (podContentType)
        {
            Color color = podContentType.uiColor;
            convertPods.ForEach(convert =>
            {
                convert.SetActive(planetManager.canPlantAtPosition(
                    podContentType,
                    convert.transform.position
                    ));
                convert.GetComponent<SpriteRenderer>().color = color;
            });
        }
    }

    void updateUI(float resources)
    {
        txtResources.text = "Resources: " + (int)resources + " / " + planetManager.ResourceCap;
    }
}
