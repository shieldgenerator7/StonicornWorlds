using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static PlanetManager Planet { get; private set; }
    public static QueueManager Queue { get; private set; }
    public static EdgeManager Edge { get; private set; }

    void Awake()
    {
        Planet = FindObjectOfType<PlanetManager>();
        Queue = FindObjectOfType<QueueManager>();
        Edge = FindObjectOfType<EdgeManager>();
    }
}
