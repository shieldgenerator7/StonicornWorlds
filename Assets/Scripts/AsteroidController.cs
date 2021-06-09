using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float exitRadius;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
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
        pods.ForEach(pod => Managers.Planet.destroyPod(pod));
        //Destroy asteroid
        Destroy(gameObject);
    }
}
