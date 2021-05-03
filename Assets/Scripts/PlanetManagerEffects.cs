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

    public TMP_Text txtResources;

    List<GameObject> goPods = new List<GameObject>();
    List<GameObject> addPods = new List<GameObject>();
    List<GameObject> convertPods = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        planetManager.onPodsListChanged += updateDisplay;
        planetManager.onPodTypeChanged += updateEdgeTypes;
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
        Color color = podType.podPrefab.GetComponent<SpriteRenderer>().color;
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
    }

    void updateUI(float resources)
    {
        txtResources.text = "Resources: " + (int)resources + " / " + planetManager.ResourceCap;
    }
}
