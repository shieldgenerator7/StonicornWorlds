using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public List<PodType> podTypeList;
    public float indexOffset = 0;

    List<Pod> pods = new List<Pod>();
    List<Vector2> addPosList = new List<Vector2>();

    public delegate void AddPosListChanged(List<Vector2> list);
    public AddPosListChanged addPosListChanged;
    public delegate void PodsListChanged(List<Pod> list);
    public PodsListChanged podsListChanged;


    // Start is called before the first frame update
    void Start()
    {
        addPosList.Add(Vector2.zero);
        addPosListChanged?.Invoke(addPosList);
        podsListChanged += (list) =>
        {
            List<Vector2> posList = list.ConvertAll(pod => pod.pos);
            addPosList = new List<Vector2>();
            foreach (Vector2 pos in posList)
            {
                List<Vector2> adjs = getAdjacentPositions(pos);
                foreach (Vector2 adj in adjs)
                {
                    if (!posList.Contains(adj) && !addPosList.Contains(adj))
                    {
                        addPosList.Add(adj);
                    }
                }
            }
            addPosListChanged?.Invoke(addPosList);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickedUI = addPosList.FirstOrDefault(addPos => checkAddPodUI(pos, addPos));
            if (clickedUI != Vector2.zero || addPosList.Count == 1)
            {
                int index = Mathf.FloorToInt(Vector2.Distance(clickedUI, Vector2.zero) + indexOffset);
                index = Mathf.Min(index, 27);
                pods.Add(new Pod(
                    clickedUI,
                    podTypeList[index]
                    ));
                podsListChanged?.Invoke(pods);
            }
        }
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

    public bool checkAddPodUI(Vector2 pos1, Vector2 pos2)
    {
        float radius = 0.5f;
        return Vector2.Distance(pos1, pos2) <= radius;
    }
}
