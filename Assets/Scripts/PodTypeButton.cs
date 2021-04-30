using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PodTypeButton : MonoBehaviour
{
    public PodType podType;
    public Image image;
    Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = podType.podPrefab.GetComponent<SpriteRenderer>();
        image.sprite = sr.sprite;
        image.color = sr.color;
        RectTransform rect = GetComponent<RectTransform>();
        position = rect.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && checkClick(Input.mousePosition))
        {
            setPodType();
        }
    }

    public bool checkClick(Vector2 pos)
    {
        return Vector2.Distance(pos, position) <= 50;
    }

    public void setPodType()
    {
        FindObjectOfType<PlanetManager>().PodType = podType;
    }

}
