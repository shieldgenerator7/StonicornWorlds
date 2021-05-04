using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDisplayer : MonoBehaviour
{
    public SpriteRenderer srLeft;
    public SpriteRenderer srMiddle;
    public SpriteRenderer srRight;

    PlanetManager planetManager;

    // Start is called before the first frame update
    void Start()
    {
        planetManager = FindObjectOfType<PlanetManager>();
        //srLeft.enabled = planetManager
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
