using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
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
        Managers.Planet.onPlanetSwapped +=
            (planet) => planet.onStateChanged += updateDisplay;
        Managers.Planet.onResourcesChanged += updateUI;
        Managers.Edge.onValidPositionListChanged += updateEditDisplay;
    }

    public void updateDisplay(Planet p)
    {
        //Update pods
        goPods.ForEach(go => Destroy(go));
        goPods.Clear();
        p.PodsAll.ForEach(pod =>
            {
                GameObject go = Instantiate(
                    pod.podType.prefab,
                    pod.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = Managers.Planet.upDir(go.transform.position);
                goPods.Add(go);
            });
    }
    public void updateDisplay(List<PodContent> podContents)
    {
        //Update pod contents
        goPodContents.ForEach(go => Destroy(go));
        goPodContents.Clear();
        podContents
            .ForEach(content =>
            {
                GameObject go = Instantiate(
                    content.contentType.prefab,
                    content.container.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = Managers.Planet.upDir(go.transform.position);
                goPodContents.Add(go);
            });
    }
    public void updateEditDisplay(List<Vector2> edges)
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
        updateEdgeTypesNew(Managers.Input.PlanetObjectType);
    }

    void updateEdgeTypesNew(PlanetObjectType pot)
    {
        if (pot is PodType pt)
        {
            updateEdgeTypes(pt);
        }
        else if (pot is PodContentType pct)
        {
            updatePlantTypes(pct);
        }
    }

    void updateEdgeTypes(PodType podType)
    {
        if (podType)
        {
            Color color = podType.uiColor;
            addPods.ForEach(add =>
            {
                add.SetActive(Managers.Planet.canBuildAtPosition(
                    podType,
                    add.transform.position
                    ));
                add.GetComponent<SpriteRenderer>().color = color;
            });
            convertPods.ForEach(convert =>
            {
                convert.SetActive(Managers.Planet.canBuildAtPosition(
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
        updateEdgeTypesNew(Managers.Input.PlanetObjectType);
    }

    void updatePlantTypes(PodContentType podContentType)
    {
        if (podContentType)
        {
            Color color = podContentType.uiColor;
            convertPods.ForEach(convert =>
            {
                convert.SetActive(Managers.Planet.canPlantAtPosition(
                    podContentType,
                    convert.transform.position
                    ));
                convert.GetComponent<SpriteRenderer>().color = color;
            });
        }
    }

    void updateUI(float resources)
    {
        txtResources.text = "Resources: " + (int)resources + " / " + Managers.Planet.ResourceCap;
    }
}
