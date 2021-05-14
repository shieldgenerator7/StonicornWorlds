using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static PlanetManager Planet { get; private set; }
    public static CameraController Camera { get; private set; }
    public static QueueManager Queue { get; private set; }
    public static EdgeManager Edge { get; private set; }
    public static InputManager Input { get; private set; }
    public static ProgressionManager Progression { get; private set; }

    public static PodTypeBank PodTypeBank { get; private set; }

    void Awake()
    {
        Planet = FindObjectOfType<PlanetManager>();
        Camera = FindObjectOfType<CameraController>();
        Queue = FindObjectOfType<QueueManager>();
        Edge = FindObjectOfType<EdgeManager>();
        Input = FindObjectOfType<InputManager>();
        Progression = FindObjectOfType<ProgressionManager>();
        PodTypeBank = FindObjectOfType<PodTypeBank>();
    }
}
