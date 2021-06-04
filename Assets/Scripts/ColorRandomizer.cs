using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public List<Color> colors;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Count)];
        Destroy(this);
    }
}
