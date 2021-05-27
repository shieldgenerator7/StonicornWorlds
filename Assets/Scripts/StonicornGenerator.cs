using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornGenerator : MonoBehaviour
{
    public List<Color> bodyColors;
    public List<Color> hairColors;

    public Stonicorn generate()
    {
        Stonicorn stonicorn = new Stonicorn();
        stonicorn.bodyColor = bodyColors[Random.Range(0, bodyColors.Count)];
        stonicorn.hairColor = hairColors[Random.Range(0, hairColors.Count)];
        return stonicorn;
    }
}
