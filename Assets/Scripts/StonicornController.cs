using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornController : MonoBehaviour
{
    public Stonicorn stonicorn;
    [SerializeField]
    private SpriteRenderer bodySR;
    [SerializeField]
    private SpriteRenderer hairSR;

    // Start is called before the first frame update
    void Start()
    {
        bodySR.color = stonicorn.bodyColor;
        hairSR.color = stonicorn.hairColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
    }
}
