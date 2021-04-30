using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMine : MonoBehaviour
{
    public float resourceGain = 1;

    PlanetManager planetManager;

    // Start is called before the first frame update
    void Start()
    {
        planetManager = FindObjectOfType<PlanetManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        planetManager.Resources += resourceGain * Time.deltaTime;
    }
}
