using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornController : MonoBehaviour
{
    public Stonicorn stonicorn;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
    }
}
