using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public GameObject addPodPrefab;

    public GameObject podPrefab;
    public List<Pod> pods = new List<Pod>();
    public List<GameObject> goPods = new List<GameObject>();
    public List<GameObject> addPods = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        updateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject clickedUI = addPods.FirstOrDefault(go => checkAddPodUI(pos, go));
            if (clickedUI)
            {
                pods.Add(new Pod(clickedUI.transform.position));
                updateDisplay();
            }
        }
    }

    public void updateDisplay()
    {
        //Update pods
        goPods.ForEach(go => Destroy(go));
        goPods.Clear();
        pods.ForEach(pod =>
        {
            GameObject go = Instantiate(
                podPrefab,
                pod.pos,
                Quaternion.identity,
                transform
                );
            goPods.Add(go);
        });
        //Update add pod UI
        addPods.ForEach(go => Destroy(go));
        addPods.Clear();
        List<Vector2> posList = pods.ConvertAll(pod => pod.pos);
        List<Vector2> edges = new List<Vector2>();
        foreach (Vector2 pos in posList)
        {
            List<Vector2> adjs = getAdjacentPositions(pos);
            foreach (Vector2 adj in adjs)
            {
                if (!posList.Contains(adj) && !edges.Contains(adj))
                {
                    edges.Add(adj);
                }
            }
        }
        if (edges.Count == 0)
        {
            edges.Add(Vector2.zero);
        }
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

    public bool checkAddPodUI(Vector2 pos, GameObject addPodUI)
    {
        SpriteRenderer sr = addPodUI.GetComponent<SpriteRenderer>();
        return sr.bounds.Contains(pos);
    }
}
