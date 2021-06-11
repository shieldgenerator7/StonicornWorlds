using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidController : PlanetProcessor
{
    public float moveSpeed = 2;
    public float exitRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        transform.position += transform.up * moveSpeed * timeDelta;
        //if missed the planet and went off screen
        if (transform.position.magnitude > exitRadius)
        {
            Destroy(gameObject);
        }
        //if hit the planet
        Pod pod = Managers.Planet.Planet.getPod(transform.position);
        if (pod)
        {
            if (pod.podType != Managers.Constants.spacePodType
                || pod.hasContentSolid())
            {
                explode();
            }
        }
    }

    void explode()
    {
        //Destroy tiles
        List<Pod> pods = Managers.Planet.Planet
            .getNeighborhood(transform.position).neighbors.ToList();
        pods.Add(Managers.Planet.Planet.getPod(transform.position));
        Managers.Planet.destroyPods(pods);
        //Destroy asteroid
        Destroy(gameObject);
    }
}
