using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public PlanetManager planetManager;

    public GameObject addPodPrefab;

    List<GameObject> goPods = new List<GameObject>();
    List<GameObject> addPods = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        planetManager.podsListChanged += updateDisplay;

        planetManager.addPosListChanged += updateAddDisplay;
    }

    public void updateDisplay(List<Pod> pods)
    {
        //Update pods
        goPods.ForEach(go => Destroy(go));
        goPods.Clear();
        pods.ForEach(pod =>
        {
            GameObject go = Instantiate(
                pod.podType.podPrefab,
                pod.pos,
                Quaternion.identity,
                transform
                );
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
    }
}
