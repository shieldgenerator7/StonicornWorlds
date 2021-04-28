using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodTypeButton : MonoBehaviour
{
    public PodType podType;

    GameObject pod;

    // Start is called before the first frame update
    void Start()
    {
        pod = Instantiate(podType.podPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && checkClick(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            FindObjectOfType<PlanetManager>().podType = podType;
        }
    }

    public bool checkClick(Vector2 pos)
    {
        SpriteRenderer sr = pod.GetComponent<SpriteRenderer>();
        return sr.bounds.Contains(pos);
    }
}
